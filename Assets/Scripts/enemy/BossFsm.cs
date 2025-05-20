using System.Collections;

using UnityEngine;

public class BossFsm : Fsm
{
	public enum FsmState
	{
		None,
		Patrol,
		Chase,
		Phase1,
		Phase2,
		Phase3,
		Dead
	}

	public FsmState CurState;
	public float ChargeForce = 40000.0f;
	public int FacingRight = 1;
	public float PatrolSpeed = 30.0f;
	public GameObject Missile;
	public GameObject Footman;
	public float PatrolDistance = 100.0f;
	public GameObject EnemyBulletPrefab;
	public GameObject LaserPrefab;
	public GameObject MissileExplodePrefab;
	
	private Animator _anim;
	private Rigidbody2D _bossRigidbody;
	private float _curPatrolDistance;
	private GameObject _curRazor;
	private float _curTimeScale;
	private Transform _eye;
	private bool _hitPlayer;
	private bool _inBattle;
	private bool _inOperation;
	private Transform _launcherLeft;
	private Transform _launcherRight;
	private GameObject _leftSide;
	private bool _leftSideCollision;
	private bool _operateDead;
	private Transform _playerTran;
	private GameObject _portal;
	private GameObject _rightSide;
	private bool _rightSideCollision;
	private Transform _shooter;
	private TimeFieldController _timeFieldController;
	private EnemyController _enemyCtrl;

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Player"))
		{
			_hitPlayer = true;
			other.collider.gameObject.GetComponent<PlayerController>().takeDamage(10,
				(other.collider.transform.position - transform.position).normalized * 100.0f);
		}
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if (other.collider.CompareTag("Player"))
		{
			_hitPlayer = false;
		}
	}

	private void OnCollisionStay2D(Collision2D other)
	{
		if (other.collider.CompareTag("Player"))
		{
			_hitPlayer = true;
		}
	}

	protected override void Initialize()
	{
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		CurState = FsmState.Patrol;
		_bossRigidbody = gameObject.GetComponent<Rigidbody2D>();
		_enemyCtrl = gameObject.GetComponent<EnemyController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);


		_anim = transform.Find("Enemy2").GetComponent<Animator>();

		_leftSide = transform.Find("LeftSide").gameObject;
		_rightSide = transform.Find("RightSide").gameObject;

		_portal = GameObject.Find("portal");
		_portal.SetActive(false);

		_eye = transform.Find("Enemy2/Body/eye");
		_shooter = transform.Find("Enemy2/Body/shooter");
		_launcherLeft = transform.Find("Enemy2/Body/LauncherLeft");
		_launcherRight = transform.Find("Enemy2/Body/LauncherRight");


		gameObject.GetComponent<EnemyController>().isBoss = true;
	}

	protected override void FsmUpdate()
	{
		_playerTran = GameObject.Find("hero").transform;
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
		GetSideCollision();

		switch (CurState)
		{
			case FsmState.Patrol: UpdatePatrolState(); break;
			case FsmState.Chase: UpdateChaseState(); break;
			case FsmState.Phase1: UpdatePhase1State(); break;
			case FsmState.Phase2: UpdatePhase2State(); break;
			case FsmState.Phase3: UpdatePhase3State(); break;
			case FsmState.Dead: UpdateDeadState(); break;
		}

		var health = _enemyCtrl.Health;
		var maxHealth = _enemyCtrl.maxHealth;
		if (_inBattle)
		{
			if (Mathf.Abs(_playerTran.position.x - transform.position.x) > 120.0f && health > 0)
			{
				CurState = FsmState.Chase;
			}
			else
			{
				if (health > maxHealth * 2 / 3)
				{
					CurState = FsmState.Phase1;
				}
				else if (health > maxHealth / 3)
				{
					CurState = FsmState.Phase2;
				}
				else if (health > 0)
				{
					CurState = FsmState.Phase3;
				}
			}
		}

		if (health <= 0)
		{
			CurState = FsmState.Dead;
		}
	}

	private void UpdatePatrolState()
	{
		_bossRigidbody.linearVelocity = Vector3.right * (PatrolSpeed * FacingRight);
		_curPatrolDistance += PatrolSpeed * Time.deltaTime;
		if ((_curPatrolDistance > PatrolDistance) | HitTheWall())
		{
			_curPatrolDistance = 0.0f;
			FacingRight *= -1;
		}

		if (Mathf.Abs(_playerTran.position.x - transform.position.x) < 130.0f)
		{
			_inBattle = true;
			CurState = FsmState.Phase1;
		}
	}

	private void UpdateChaseState()
	{
		float facing = Mathf.Sign(_playerTran.position.x - transform.position.x);
		_bossRigidbody.linearVelocity = Vector3.right * (PatrolSpeed * facing);
	}

	private void UpdatePhase1State()
	{
		if (_inOperation)
		{
			return;
		}

		if (transform.position.y > _playerTran.position.y)
		{
			if (Mathf.Abs(_playerTran.position.x - transform.position.x) > 90.0f)
			{
				_inOperation = true;
				ChargeAttack();
			}
			else
			{
				_inOperation = true;
				LaserAttack();
			}
		}
		else
		{
			_inOperation = true;
			BarrageAttack();
		}
	}

	private void UpdatePhase2State()
	{
		if (!_inOperation)
		{
			if (transform.position.y > _playerTran.position.y)
			{
				if (Mathf.Abs(_playerTran.position.x - transform.position.x) > 90.0f)
				{
					_inOperation = true;
					SummonAttack();
				}
				else
				{
					_inOperation = true;
					LaserAttack();
				}
			}
			else
			{
				if (Random.value < 0.5)
				{
					_inOperation = true;
					BarrageAttack();
				}
				else
				{
					_inOperation = true;
					ShootAttack();
				}
			}
		}
	}

	private void UpdatePhase3State()
	{
		if (_inOperation)
		{
			return;
		}

		if (Random.value < 0.4)
		{
			_inOperation = true;
			MissileAttack();
		}
		else
		{
			if (transform.position.y > _playerTran.position.y)
			{
				_inOperation = true;
				SummonAttack();
			}
			else
			{
				if (Random.value < 0.5)
				{
					_inOperation = true;
					BarrageAttack();
				}
				else
				{
					_inOperation = true;
					ShootAttack();
				}
			}
		}
	}

	private void UpdateDeadState()
	{
		if (!_inOperation && !_operateDead)
		{
			StartCoroutine(BossDead());
			_operateDead = true;
		}
	}

	private void ShootAttack()
	{
		float facing = Mathf.Sign(_playerTran.position.x - transform.position.x);
		StartCoroutine(Shoot(facing));
	}

	private IEnumerator BossDead()
	{
		StartCoroutine(ExplodeItself());
		yield return new WaitForSeconds(3.0f);
		_anim.SetBool("isDead", true);
		yield return 0;
	}

	private IEnumerator Shoot(float facing)
	{
		//tracing shoot
		for (int times = 0; times < 10; times++)
		{
			Instantiate(EnemyBulletPrefab, _shooter.position,
				Quaternion.FromToRotation(Vector3.right, _playerTran.position - _shooter.position));
			yield return new WaitForSeconds(0.3f / _curTimeScale);
		}

		_inOperation = false;
	}

	private void BarrageAttack()
	{
		float facing = Mathf.Sign(_playerTran.position.x - transform.position.x);
		StartCoroutine(Barrage(facing));
	}

	private IEnumerator Barrage(float facing)
	{
		//barrage
		for (int times = 0; times < 3; times++)
		{
			for (float radius = 0.0f; radius < 60.0f; radius += 6.0f)
			{
				Instantiate(EnemyBulletPrefab, _shooter.position,
					Quaternion.FromToRotation(Vector3.right, new Vector3(facing, Mathf.Tan(radius - 30.0f))));
			}

			yield return new WaitForSeconds(0.8f / _curTimeScale);
		}

		_inOperation = false;
	}

	private void ChargeAttack()
	{
		float facing = Mathf.Sign(_playerTran.position.x - transform.position.x);
		StartCoroutine(Charge(facing));
	}

	private IEnumerator Charge(float facing)
	{
		float tempChargeForce = ChargeForce;
		_anim.SetTrigger("preCharge");
		yield return new WaitForSeconds(1.0f); //给动画留的时间

		while (!HitTheWall() && !_hitPlayer)
		{
			_bossRigidbody.AddForce(Vector3.right * (tempChargeForce * facing));
			yield return 0;
		}

		while (_bossRigidbody.linearVelocity.magnitude > 1.0f)
		{
			yield return 0;
		}

		for (float timer = 1.5f; timer > 0; timer -= Time.deltaTime)
		{
			_bossRigidbody.linearVelocity = Vector3.right * (PatrolSpeed * facing * -1);
			yield return 0;
		}

		_inOperation = false;
	}

	private void LaserAttack()
	{
		float facing = Mathf.Sign(_playerTran.position.x - transform.position.x);
		StartCoroutine(LaserAtk(facing));
	}

	private IEnumerator LaserAtk(float facing)
	{
		_anim.SetTrigger("preLazor");
		yield return new WaitForSeconds(1.0f); //给动画留的时间
		_curRazor = Instantiate(LaserPrefab, _eye.transform.position,
			Quaternion.FromToRotation(Vector3.forward, new Vector3(1.0f * facing, -2.0f, 0.0f)));
		for (float timer = 3.0f; timer > 0; timer -= Time.deltaTime)
		{
			_curRazor.transform.position = _eye.transform.position;
			_curRazor.transform.eulerAngles = new Vector3(_curRazor.transform.eulerAngles.x - (15.0f * Time.deltaTime),
				_curRazor.transform.eulerAngles.y, _curRazor.transform.eulerAngles.z);
			yield return 0;
		}

		Destroy(_curRazor);
		_inOperation = false;
	}

	private void MissileAttack()
	{
		StartCoroutine(MissileLaunch());
	}

	private IEnumerator MissileLaunch()
	{
		_anim.SetTrigger("preMissile");
		yield return new WaitForSeconds(1.0f);
		for (int i = 0; i < 2; i++)
		{
			Instantiate(Missile, new Vector3(_launcherLeft.position.x, _launcherLeft.position.y, 0.0f),
				Quaternion.FromToRotation(Vector3.left, Vector3.up));
			Instantiate(Missile, new Vector3(_launcherRight.position.x, _launcherRight.position.y, 0.0f),
				Quaternion.FromToRotation(Vector3.left, Vector3.up));
			yield return new WaitForSeconds(1.0f);
		}

		_inOperation = false;
	}

	private void SummonAttack()
	{
		StartCoroutine(Summon());
	}

	private IEnumerator Summon()
	{
		_anim.SetTrigger("preSummon");
		yield return new WaitForSeconds(1.0f);
		while (_bossRigidbody.linearVelocity.magnitude > 1.0f)
		{
			yield return 0;
		}

		for (int i = 0; i < 2; i++)
		{
			GameObject rightFootman = Instantiate(Footman,
				transform.position + (Vector3.up * 80.0f) + (Vector3.right * 50.0f), transform.rotation);
			GameObject leftFootman = Instantiate(Footman,
				transform.position + (Vector3.up * 80.0f) + (Vector3.left * 50.0f), transform.rotation);
			rightFootman.GetComponent<FootmanController>().SetQuickExplode(-1);
			leftFootman.GetComponent<FootmanController>().SetQuickExplode(1);
			yield return new WaitForSeconds(3.0f);
		}

		_inOperation = false;
	}

	private IEnumerator ExplodeItself()
	{
		_anim.SetTrigger("explode");
		for (int i = 15; i > 0; i--)
		{
			Instantiate(MissileExplodePrefab,
				transform.position + new Vector3(Random.Range(-30.0f, 30.0f), Random.Range(-30.0f, 20.0f), 0.0f),
				transform.rotation);
			yield return new WaitForSeconds(0.3f);
		}

		for (int i = 30; i > 0; i--)
		{
			Instantiate(MissileExplodePrefab,
				transform.position + new Vector3(Random.Range(-30.0f, 30.0f), Random.Range(-30.0f, 25.0f), 0.0f),
				transform.rotation);
		}

		Destroy(gameObject);
		_portal.SetActive(true);
		yield return 0;
	}

	private void GetSideCollision()
	{
		_leftSideCollision = Physics2D.Linecast(transform.position, _leftSide.transform.position,
			(1 << LayerMask.NameToLayer("Platforms")) | (1 << LayerMask.NameToLayer("OneWayPlatforms")));
		_rightSideCollision = Physics2D.Linecast(transform.position, _rightSide.transform.position,
			(1 << LayerMask.NameToLayer("Platforms")) | (1 << LayerMask.NameToLayer("OneWayPlatforms")));
	}

	private bool HitTheWall()
	{
		return _leftSideCollision || _rightSideCollision;
	}
}