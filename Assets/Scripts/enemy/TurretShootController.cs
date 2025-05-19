using UnityEngine;
using UnityEngine.Serialization;

public class TurretShootController : MonoBehaviour
{
	[FormerlySerializedAs("enemybullet")]
	public GameObject EnemyBulletPrefab;
	[FormerlySerializedAs("searchRadius")]
	public float SearchRadius = 120.0f;
	[FormerlySerializedAs("shootInterval")]
	public float ShootInterval = 0.8f;
	
	private Vector3 _facingDirection = new(0.0f, -1.0f, 0.0f);
	private float _curTimeScale;
	private float _distance;
	private Transform _headTran;
	private float _nextShoot;

	private Transform _playerTran;
	private Vector3 _predictPosition;
	private float _scaledTime;

	private TimeFieldController _timeFieldController;

	private void Start()
	{
		_playerTran = FindPlayerTran();
		_predictPosition = _playerTran.position;
		_headTran = transform;
		_scaledTime = Time.time;
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
	}

	private void Update()
	{
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
		_distance = Vector3.Distance(_playerTran.position, transform.position);
		_predictPosition = _playerTran.position;
		_facingDirection = _predictPosition - transform.position;

		_scaledTime += Time.deltaTime * _curTimeScale;
		if (!(_distance < SearchRadius))
		{
			return;
		}

		_headTran.rotation = Quaternion.FromToRotation(Vector3.right, _facingDirection);
		if (Physics2D.Linecast(transform.position, _playerTran.position, 1 << LayerMask.NameToLayer("Platforms")))
		{
			return;
		}

		if (_scaledTime > _nextShoot)
		{
			Instantiate(EnemyBulletPrefab, transform.position,
				Quaternion.FromToRotation(Vector3.right, _facingDirection));
			_nextShoot = Time.time + ShootInterval;
			_scaledTime = Time.time;
		}
	}

	private Transform FindPlayerTran()
	{
		var players = GameObject.FindGameObjectsWithTag("Player");
		if (players.Length == 1)
		{
			return players[0].transform;
		}

		return null;
	}
}