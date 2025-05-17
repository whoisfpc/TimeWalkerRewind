using System;

using UnityEngine;

public class BulletController : MonoBehaviour
{
	public float speed = 300.0f;
	public float lifetime = 5.0f;
	public GameObject bursteffect;
	public GameObject Source { get; set; } // who shoot the bullet

	private float _life;
	private TimeFieldController _timeFieldController;
	private float _trueSpeed;
	private static readonly Collider2D[] AllTouches = new Collider2D[8];

	private void Start()
	{
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_trueSpeed = speed * _timeFieldController.getTimescale(transform.position);
	}

	private void FixedUpdate()
	{
		_trueSpeed = speed * _timeFieldController.getTimescale(transform.position);
		transform.Translate(Vector2.right * (_trueSpeed * Time.deltaTime));

		_life += Time.deltaTime;
		if (_life > lifetime)
		{
			Destroy(gameObject);
		}

		// TODO: 不要基于名字判定，子弹逻辑需要改为射线检测
		if (gameObject.name.Equals("enemybullet(Clone)"))
		{
			if (Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Platforms")))
			{
				Instantiate(bursteffect, transform.position, Quaternion.FromToRotation(Vector3.up, transform.forward));
				Destroy(gameObject);
			}

			Collider2D playerCol =
				Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Player"));
			if (playerCol)
			{
				playerCol.GetComponent<PlayerController>()
					.takeDamage(10, (transform.right * 5000.0f) + (Vector3.up * 2000.0f));
				Instantiate(bursteffect, transform.position, Quaternion.FromToRotation(Vector3.up, transform.forward));
				Destroy(gameObject);
			}
		}

		if (gameObject.name.Equals("bullet(Clone)"))
		{
			if (Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Platforms")))
			{
				Instantiate(bursteffect, transform.position, Quaternion.FromToRotation(Vector3.up, transform.forward));
				Destroy(gameObject);
			}

			var size = Physics2D.OverlapCircleNonAlloc(transform.position, 0.1f, AllTouches, 1 << LayerMask.NameToLayer("Enemies"));
			if (size > 0)
			{
				AllTouches[0].gameObject.GetComponent<EnemyController>().takeDamage(10,
					(transform.right * 5000.0f) + (Vector3.up * 2000.0f), Source);
				Instantiate(bursteffect, transform.position, Quaternion.FromToRotation(Vector3.up, transform.forward));
				Destroy(gameObject);
			}
		}
	}
}