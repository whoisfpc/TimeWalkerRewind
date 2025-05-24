using System.Collections;

using UnityEngine;

public class ShootController : MonoBehaviour
{
	private const string FireKeyStr = "Fire";
	
	public GameObject bullet;
	public GameObject gun;

	[SerializeField]
	private float _fireInterval = 0.2f;
	private float EffectiveFireInterval => _fireInterval / _fireRateScale;

	private float _fireRateScale = 1f;
	private Transform _gunTran;
	private Camera _mainCamera;
	private float _nextFire;

	private void Start()
	{
		_mainCamera = Camera.main;
	}

	private void Update()
	{
		bool isDead = GetComponent<PlayerController>().HasDead();
		if (MenuManager.Instance.Paused || isDead)
		{
			return;
		}

		if (!(Time.time > _nextFire))
		{
			return;
		}

		if (Input.GetButton(FireKeyStr))
		{
			Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0.0f;
			_gunTran = gun.transform;
			GameObject bulletInst = Instantiate(bullet, _gunTran.position,
				Quaternion.FromToRotation(Vector3.right, mousePosition - _gunTran.position));
			bulletInst.GetComponent<BulletController>().Setup(gameObject, true);

			_nextFire = Time.time + EffectiveFireInterval;
		}
	}

	public void ShootSpeedAccelerate(int scale, float duration)
	{
		StartCoroutine(ShootSpeedAcc(scale, duration));
	}

	private IEnumerator ShootSpeedAcc(int scale, float duration)
	{
		_fireRateScale = scale;
		yield return new WaitForSeconds(duration);
		_fireRateScale = 1f;
	}
}