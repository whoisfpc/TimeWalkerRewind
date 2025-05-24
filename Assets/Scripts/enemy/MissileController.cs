using UnityEngine;
using UnityEngine.Serialization;

public class MissileController : MonoBehaviour
{
	private const float Accelerate = 30.0f;
	private const float MaxSpeed = 80.0f;
	private const float RotateSpeed = 2.0f;
	private const int MissileDamage = 10;
	
	[FormerlySerializedAs("missileExplode")]
	public GameObject MissileExplode;

	private float _curTimeScale;
	private Transform _playerTran;
	private float _speed = 50.0f;
	private TimeFieldController _timeFieldController;
	private float _timer;

	private void Start()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		int tempIndex = Random.Range(0, players.Length);
		_playerTran = players[tempIndex].transform;


		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
	}

	private void Update()
	{
		_timer += Time.deltaTime;
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
		if (_speed < MaxSpeed)
		{
			_speed += Accelerate * Time.deltaTime;
		}

		if (_timer < 0.5f)
		{
			transform.Translate(-transform.right * (_speed * _curTimeScale * Time.deltaTime), Space.World);
		}
		else
		{
			transform.Translate(-transform.right * (_speed * _curTimeScale * Time.deltaTime), Space.World);
			transform.rotation = Quaternion.Slerp(transform.rotation,
				Quaternion.FromToRotation(Vector3.left, _playerTran.position - transform.position),
				RotateSpeed * _curTimeScale * Time.deltaTime);
		}

		if (Physics2D.OverlapCircle(transform.position, 3.0f,
			    (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Platforms"))))
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		Explode();
	}

	private void Explode()
	{
		Instantiate(MissileExplode, transform.position, transform.rotation);

		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10.0f, 1 << LayerMask.NameToLayer("Player"));
		foreach (var hit in hits)
		{
			hit.gameObject.GetComponent<PlayerController>().TakeDamage(MissileDamage, -transform.right * 100.0f);
		}
	}
}