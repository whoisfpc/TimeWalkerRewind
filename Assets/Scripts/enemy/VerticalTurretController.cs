using UnityEngine;

public class VerticalTurretController : MonoBehaviour
{
	[SerializeField]
	private GameObject _laserPrefab;
	[SerializeField]
	private float _activateTime = 3.0f;

	private bool _active = true;
	private GameObject _curLaser;
	private float _curTimeScale;

	private TimeFieldController _timeFieldController;
	private float _timer;

	private void Start()
	{
		_curLaser = Instantiate(_laserPrefab, transform.position + (transform.up * 5.0f),
			Quaternion.FromToRotation(Vector3.forward, transform.up));

		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
	}

	private void Update()
	{
		_curTimeScale = _timeFieldController.getTimescale(transform.position);

		_timer += Time.deltaTime * _curTimeScale;

		if (_active)
		{
			_curLaser.SetActive(true);
			if (_timer > _activateTime)
			{
				_timer = 0;
				_active = false;
			}
		}
		else
		{
			_curLaser.SetActive(false);
			if (_timer > _activateTime)
			{
				_timer = 0;
				_active = true;
			}
		}
	}
}