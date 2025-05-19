using UnityEngine;

public class LazerTurretController : MonoBehaviour
{
	public GameObject LaserPrefab;
	public float MaxAngle = 90.0f;
	public float RotateSpeed = 20.0f;
	public float InitAngle;
	
	private float _curAngle;
	private GameObject _curLaser;
	private float _curTimeScale;
	private int _face = 1;

	private TimeFieldController _timeFieldController;

	private void Start()
	{
		_curLaser = Instantiate(LaserPrefab, transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.down));
		_curAngle = InitAngle;

		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
	}

	private void Update()
	{
		_curTimeScale = _timeFieldController.getTimescale(transform.position);

		_curAngle += RotateSpeed * _curTimeScale * Time.deltaTime * _face;
		_curLaser.transform.Rotate(Vector3.up * (RotateSpeed * _curTimeScale * _face * Time.deltaTime));
		if (_curAngle > MaxAngle / 2 || _curAngle < -MaxAngle / 2)
		{
			_face *= -1;
		}
	}
}