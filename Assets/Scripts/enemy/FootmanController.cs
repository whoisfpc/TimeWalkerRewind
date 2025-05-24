using System.Collections;

using UnityEngine;
using UnityEngine.Serialization;

public class FootmanController : MonoBehaviour
{
	private static readonly int IsDeadParamHash = Animator.StringToHash("isdead");
	private static readonly int SpeedParamHash = Animator.StringToHash("Speed");

	[FormerlySerializedAs("maxSpeed")]
	public float MaxSpeed = 30.0f;
	[FormerlySerializedAs("footmanExplode")]
	public GameObject FootmanExplodePrefab;
	
	private int _face = -1;
	private Animator _anim;
	private float _curTimeScale;
	private GameObject _leftSide;
	private bool _leftSideCollision;
	private bool _quickExplode;
	private GameObject _rightSide;
	private bool _rightSideCollision;
	private bool _startMoving;
	private float _tempSpeed;
	private TimeFieldController _timeFieldController;
	private Rigidbody2D _rb2d;

	private void Start()
	{
		_rb2d = GetComponent<Rigidbody2D>();
		_anim = GetComponentInChildren<Animator>();
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);


		_leftSide = transform.Find("LeftSide").gameObject;
		_rightSide = transform.Find("RightSide").gameObject;
	}

	private void Update()
	{
		_curTimeScale = _timeFieldController.getTimescale(transform.position);

		if (_startMoving && _rb2d.linearVelocity.magnitude < 0.1f)
		{
			_face *= -1;
		}

		_rb2d.linearVelocity = new Vector2(_tempSpeed * _face, _rb2d.linearVelocity.y) * _curTimeScale;
		_anim.SetFloat(SpeedParamHash, _rb2d.linearVelocity.magnitude);
		_anim.speed = _curTimeScale;
		GetSideCollision();
		FindPlayerNearby();

		if (_leftSideCollision)
		{
			if (!_quickExplode)
			{
				_face = 1;
			}
			else
			{
				ExplodeItSelf();
			}
		}

		if (_rightSideCollision)
		{
			if (!_quickExplode)
			{
				_face = -1;
			}
			else
			{
				ExplodeItSelf();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("map") && !_startMoving)
		{
			_startMoving = true;
			_tempSpeed = MaxSpeed;
		}
	}

	private void FindPlayerNearby()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players)
		{
			Vector3 playerPos = player.transform.position;
			if (Vector3.Distance(playerPos, transform.position) < 20.0f)
			{
				MaxSpeed = 0;
				_anim.SetBool(IsDeadParamHash, true);
				StartCoroutine(Explode());
			}
		}
	}

	private IEnumerator Explode()
	{
		_tempSpeed = 0.0f;
		for (float timer = 1.5f; timer > 0.0f; timer -= Time.deltaTime * _curTimeScale)
		{
			yield return 0;
		}

		Instantiate(FootmanExplodePrefab, transform.position, transform.rotation);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players)
		{
			Vector3 playerPos = player.transform.position;
			if (Vector3.Distance(playerPos, transform.position) < 30.0f)
			{
				player.GetComponent<PlayerController>()
					.TakeDamage(40, (playerPos - transform.position).normalized * 2000.0f);
			}
		}

		Destroy(gameObject);
	}

	private void GetSideCollision()
	{
		_leftSideCollision = Physics2D.Linecast(transform.position, _leftSide.transform.position,
			(1 << LayerMask.NameToLayer("Platforms")) | (1 << LayerMask.NameToLayer("OneWayPlatforms")));
		_rightSideCollision = Physics2D.Linecast(transform.position, _rightSide.transform.position,
			(1 << LayerMask.NameToLayer("Platforms")) | (1 << LayerMask.NameToLayer("OneWayPlatforms")));
	}

	private void ExplodeItSelf()
	{
		_anim.SetBool(IsDeadParamHash, true);
		StartCoroutine(Explode());
	}

	public void SetQuickExplode(int tempFace)
	{
		_quickExplode = true;
		_face *= tempFace;
	}
}