using System;
using System.Collections.Generic;

using UnityEngine;

public class BulletController : MonoBehaviour
{
	private static readonly List<Collider2D> AllTouches = new(8);
	
	public float speed = 300.0f;
	public float lifetime = 5.0f;
	public GameObject bursteffect;

	private float _life;
	private TimeFieldController _timeFieldController;
	private float _trueSpeed;
	private GameObject _source; // who shoot the bullet
	private bool _isPlayerBullet;

	public void Setup(GameObject source, bool isPlayerBullet)
	{
		_source = source;
		_isPlayerBullet = isPlayerBullet;
	}

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

		if (!_isPlayerBullet)
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
		else 
		{
			// player shoot bullet
			if (Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Platforms")))
			{
				Instantiate(bursteffect, transform.position, Quaternion.FromToRotation(Vector3.up, transform.forward));
				Destroy(gameObject);
			}

			ContactFilter2D filter = new();
			filter.NoFilter();
			filter.SetLayerMask(1 << LayerMask.NameToLayer("Enemies"));
			var size = Physics2D.OverlapCircle(transform.position, 0.1f, filter, AllTouches);
			if (size > 0)
			{
				AllTouches[0].gameObject.GetComponent<EnemyController>().TakeDamage(10,
					(transform.right * 5000.0f) + (Vector3.up * 2000.0f), _source);
				Instantiate(bursteffect, transform.position, Quaternion.FromToRotation(Vector3.up, transform.forward));
				Destroy(gameObject);
			}
		}
	}
}