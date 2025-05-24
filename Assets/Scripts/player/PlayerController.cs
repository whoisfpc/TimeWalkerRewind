using System.Collections;

using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
	private static readonly int CrouchParamHash = Animator.StringToHash("Crouch");
	private static readonly int GroundParamHash = Animator.StringToHash("Ground");
	private static readonly int ForwardParamHash = Animator.StringToHash("Forward");
	private static readonly int SpeedParamHash = Animator.StringToHash("Speed");
	private static readonly int VSpeedParamHash = Animator.StringToHash("vSpeed");
	private static readonly int IsDeadParamHash = Animator.StringToHash("isdead");

	/* input name for controller */
	private const string HorizontalStr = "Horizontal";
	private const string VerticalStr = "Vertical";
	private const string JumpStr = "Jump";
	private const string LightStr = "Light";
	private const string TimeSlowStr = "TimeSlow";
	private const string TimeBombStr = "TimeBomb";
	private const string TimeBackStr = "TimeBack";

	public Transform GunTran;
	public GameObject GunLight;
	public GameObject TimeField;
	public GameObject TimeBombProto;

	/* player property */
	public float MaxHealth = 100;
	public float MaxEnergy = 100f;
	public float MaxSpeed = 60f;
	public float JumpForce = 12000f;
	public int MaxJumpTime = 2;
	[Range(0, 1)]
	public float BackSpeed = 0.7f; // backward walk speed percent
	public float EnergyRestorePerSecond = 2f;
	public float MaxRestoreEnergy = 30f;

	/* skill attribute */
	public float TimeSlowEnergyPerSecond = 5f;
	public float TimeSlowStartUpEnergy = 10f;
	public float BackToPastTime = 2f;
	public float BackCd = 5f;
	public float BackToPastEnergy = 30f;
	public float TimeBombEnergy = 33f;
	public float TimeBombCd = 10f;
	
	public bool FacingRight { get; private set; } = true;

	public bool IsDead { get; private set; }

	private Rigidbody2D _rb2d;
	private Animator _anim; 
	private readonly ArrayList _backPositions = new();
	private bool _canBeDamaged = true;
	private float _canBeDamagedTimer;
	private float _curEnergy = 100f;

	private float _curHealth = 100;

	private Transform _groundCheck; // A position marking where to check if the player is grounded.
	private bool _grounded; // Whether the player is grounded.

	private bool _isPause;
	private bool _jump;

	private int _jumpTime;
	private GameObject _cape;
	private readonly SkillState _timeBackState = new("TimeBack");
	private readonly SkillState _timeBombState = new("TimeBomb");
	private readonly SkillState _timeSlowState = new("TimeSlow");

	private void Awake()
	{
		_rb2d = GetComponent<Rigidbody2D>();
		_groundCheck = transform.Find("groundCheck");
		_anim = transform.Find("player").GetComponent<Animator>();
	}

	private void Start()
	{
		_cape = transform.Find("cape").gameObject;

		_curHealth = MaxHealth;
		_curEnergy = MaxEnergy;
	}

	private void Update()
	{
		_isPause = MenuManager.Instance.Paused;

		// test to determine whether player is on ground
		int groundLayerMask = (1 << LayerMask.NameToLayer("Platforms")) |
		                      (1 << LayerMask.NameToLayer("OneWayPlatforms")) |
		                      (1 << LayerMask.NameToLayer("Enemies")) | (1 << LayerMask.NameToLayer("Player"));
		Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, 2, groundLayerMask);
		_grounded = false;
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				_grounded = true;
			}
		}


		if (_isPause || IsDead)
		{
			return;
		}

		/* all input response will disable when game is paused */
		if (Input.GetButtonDown(JumpStr))
		{
			_jump = true;
		}

		if (_grounded)
		{
			_jumpTime = MaxJumpTime;
		}

		if (!_grounded && _jumpTime == MaxJumpTime)
		{
			_jumpTime--;
		}

		if (Input.GetAxis(VerticalStr) < 0)
		{
			_anim.SetBool(CrouchParamHash, true);
		}
		else
		{
			if (!Physics2D.Linecast(transform.position, transform.Find("headCheck").position,
				    1 << LayerMask.NameToLayer("Platforms")))
			{
				_anim.SetBool(CrouchParamHash, false);
			}
		}

		if (Input.GetAxis(VerticalStr) < 0 && Physics2D.Linecast(transform.position, _groundCheck.position,
			    1 << LayerMask.NameToLayer("OneWayPlatforms")))
		{
			Collider2D[] tempOneWayPlatform = Physics2D.OverlapCircleAll(_groundCheck.position, 10.0f,
				1 << LayerMask.NameToLayer("OneWayPlatforms"));
			for (int i = 0; i < tempOneWayPlatform.Length; i++)
			{
				tempOneWayPlatform[i].transform.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
			}
		}

		// Automatic energy recover
		if (_curEnergy < MaxRestoreEnergy)
		{
			_curEnergy += EnergyRestorePerSecond * Time.deltaTime;
			if (_curEnergy > MaxRestoreEnergy)
			{
				_curEnergy = MaxRestoreEnergy;
			}
		}

		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0.0f;
		Vector3 lookDir = mousePosition - transform.position;
		float lookRight = lookDir.x;

		if (IsDead)
		{
			lookRight = 1;
		}

		if (lookRight > 0 && !FacingRight)
		{
			Cloth cl = _cape.gameObject.GetComponent<Cloth>();
			CapsuleCollider[] oldCa = cl.capsuleColliders;
			cl.capsuleColliders = new CapsuleCollider[0];
			Flip();
			cl.capsuleColliders = oldCa;
		}
		else if (lookRight < 0 && FacingRight)
		{
			Cloth cl = _cape.gameObject.GetComponent<Cloth>();
			CapsuleCollider[] oldCa = cl.capsuleColliders;
			cl.capsuleColliders = new CapsuleCollider[0];
			Flip();
			cl.capsuleColliders = oldCa;
		}

		/* Deal with skill input */

		// open or close light
		if (Input.GetButtonDown(LightStr))
		{
			GunLight.SetActive(!GunLight.activeSelf);
		}

		// ope or close time slow field
		if (Input.GetButtonDown(TimeSlowStr))
		{
			SwitchTimeSlow();
		}

		// Continuous consume energy while time slow field is opening
		if (_timeSlowState.OnUsing)
		{
			if (!UseEnergy(TimeSlowEnergyPerSecond * Time.deltaTime))
			{
				TimeField.SetActive(false);
				_curEnergy = 0;
				_timeSlowState.OnUsing = false;
			}
		}

		_timeSlowState.LackEnergy = _curEnergy < TimeSlowStartUpEnergy;
		_timeSlowState.CanUse = !_timeSlowState.LackEnergy && _timeSlowState.RemainCd == 0;

		// try to shoot time bomb
		_timeBombState.RemainCd = Mathf.Max(0, _timeBombState.RemainCd - Time.deltaTime);
		if (Input.GetButtonDown(TimeBombStr))
		{
			ShootTimeBomb();
		}

		_timeBombState.LackEnergy = _curEnergy < TimeBombEnergy;
		_timeBombState.CanUse = !_timeBombState.LackEnergy && _timeBombState.RemainCd == 0;

		// try to use time back
		_timeBackState.RemainCd = Mathf.Max(0, _timeBackState.RemainCd - Time.deltaTime);
		if (Input.GetButtonDown(TimeBackStr))
		{
			UseTimeBack();
		}

		_timeBackState.LackEnergy = _curEnergy < BackToPastEnergy;
		_timeBackState.CanUse = !_timeBackState.LackEnergy && _timeBackState.RemainCd == 0;
	}

	private void FixedUpdate()
	{
		if (_isPause || IsDead)
		{
			return;
		}

		_anim.SetBool(GroundParamHash, _grounded);
		float h = Input.GetAxis(HorizontalStr);
		if (_isPause)
		{
			h = 0;
		}

		bool forward;
		if ((FacingRight && h > 0) || (!FacingRight && h < 0))
		{
			forward = true;
		}
		else
		{
			forward = false;
		}

		_anim.SetBool(ForwardParamHash, forward);
		float temp = 1;
		if (!forward)
		{
			temp = BackSpeed;
		}

		_rb2d.linearVelocity = new Vector2(h * temp * MaxSpeed, _rb2d.linearVelocity.y);
		_anim.SetFloat(SpeedParamHash, Mathf.Abs(_rb2d.linearVelocity.x * h));
		if (_jumpTime > 0 && _jump)
		{
			_rb2d.linearVelocity = new Vector2(_rb2d.linearVelocity.x, 0.0f);
			_rb2d.AddForce(Vector2.up * JumpForce);
			_jumpTime--;
			_jump = false;
		}

		if (_backPositions.Count >= BackToPastTime * 50)
		{
			_backPositions.RemoveAt(0);
		}

		_backPositions.Add(transform.position);
		_anim.SetFloat(VSpeedParamHash, _rb2d.linearVelocity.y / 20.0f);
		if (!_canBeDamaged)
		{
			_canBeDamagedTimer += Time.deltaTime;
			if (_canBeDamagedTimer > 1.0f)
			{
				_canBeDamagedTimer = 0.0f;
				_canBeDamaged = true;
			}
		}
	}

	private void Flip()
	{
		FacingRight = !FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void UseTimeBack()
	{
		if (_timeBackState.CanUse && UseEnergy(BackToPastEnergy))
		{
			transform.Translate((Vector3)_backPositions[0] - transform.position);
			_timeBackState.RemainCd = BackCd;
		}
	}

	private void SwitchTimeSlow()
	{
		if (!_timeSlowState.OnUsing)
		{
			if (UseEnergy(TimeSlowStartUpEnergy))
			{
				_timeSlowState.OnUsing = true;
				TimeField.SetActive(true);
			}
		}
		else
		{
			_timeSlowState.OnUsing = false;
			TimeField.SetActive(false);
		}
	}


	private void ShootTimeBomb()
	{
		if (_timeBombState.CanUse)
		{
			if (UseEnergy(TimeBombEnergy))
			{
				Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePosition.z = 0.0f;
				GameObject curBomb = Instantiate(TimeBombProto, GunTran.position,
					Quaternion.FromToRotation(Vector3.right, mousePosition - GunTran.position));
				curBomb.GetComponent<Rigidbody2D>()
					.AddForce((mousePosition - GunTran.position).normalized * 5000.0f);

				_timeBombState.RemainCd = TimeBombCd;
			}
		}
	}

	public float GetCurHealth()
	{
		return _curHealth;
	}

	public float GetCurEnergy()
	{
		return _curEnergy;
	}

	public void TakeDamage(int damage, Vector3 force)
	{
		if (IsDead)
		{
			return;
		}

		if (_canBeDamaged)
		{
			_curHealth -= damage;
			_rb2d.AddForce(force);
			_canBeDamaged = false;
		}

		if (_curHealth <= 0)
		{
			_curHealth = 0;
			IsDead = true;
			if (_timeSlowState.OnUsing)
			{
				SwitchTimeSlow();
			}

			_cape.gameObject.SetActive(false);
			_anim.SetTrigger(IsDeadParamHash);
		}
	}

	public void RestoreHealth(int restore)
	{
		_curHealth = Mathf.Min(restore + _curHealth, MaxHealth);
	}

	public void RestoreEnergy(int restore)
	{
		_curEnergy = Mathf.Min(restore + _curEnergy, MaxEnergy);
	}

	private bool UseEnergy(float energy)
	{
		if (_curEnergy >= energy)
		{
			_curEnergy -= energy;
			return true;
		}

		return false;
	}

	public SkillState GetSkillState(string skillName)
	{
		return skillName switch
		{
			"TimeSlow" => _timeSlowState,
			"TimeBack" => _timeBackState,
			"TimeBomb" => _timeBombState,
			_ => null
		};
	}

	public bool HasDead()
	{
		return IsDead;
	}
}

public class SkillState
{
	public bool CanUse;
	public bool LackEnergy;
	public bool OnUsing;
	public float RemainCd;
	public string SkillName;

	public SkillState(string skillName)
	{
		this.SkillName = skillName;
		RemainCd = 0;
		OnUsing = false;
		CanUse = true;
		LackEnergy = false;
	}
}