using System.Collections;

using UnityEngine;

public class TrapBomb : MonoBehaviour
{
	public Sprite preBoomSprite;
	public float boomDelay = 0.3f;
	public float boomArea = 30f;
	public GameObject footmanExplode;
	
	private float _curTimeScale;
	private float _formerTimeScale;
	private AudioSource _preBoomSound;
	private TimeFieldController _timeFieldController;
	private Rigidbody2D _rb2d;

	private void Start()
	{
		_preBoomSound = GetComponent<AudioSource>();
		_formerTimeScale = 1.0f;
		_curTimeScale = 1.0f;
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
		_rb2d = GetComponent<Rigidbody2D>();
		_rb2d.gravityScale = _curTimeScale;
		_rb2d.linearVelocity = _rb2d.linearVelocity * _curTimeScale / _formerTimeScale;
		_rb2d.angularVelocity = _rb2d.angularVelocity * _curTimeScale / _formerTimeScale;
		_formerTimeScale = _curTimeScale;
	}

	private void Update()
	{
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
		_rb2d.gravityScale = _curTimeScale;
		_rb2d.linearVelocity = _rb2d.linearVelocity * _curTimeScale / _formerTimeScale;
		_rb2d.angularVelocity = _rb2d.angularVelocity * _curTimeScale / _formerTimeScale;
		_formerTimeScale = _curTimeScale;
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.CompareTag("Player"))
		{
			StartCoroutine(Boom(coll.gameObject));
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, boomArea);
	}

	private IEnumerator Boom(GameObject player)
	{
		_preBoomSound.Play();
		GetComponent<SpriteRenderer>().sprite = preBoomSprite;
		for (float timer = boomDelay; timer > 0.0f; timer -= Time.deltaTime * _curTimeScale)
		{
			yield return 0;
		}

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (var p in players)
		{
			if (Vector3.Distance(p.transform.position, transform.position) < boomArea)
			{
				p.GetComponent<PlayerController>().TakeDamage(10, Vector3.zero);
			}
		}

		Instantiate(footmanExplode, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}