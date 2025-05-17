using System.Collections;

using UnityEngine;

public class MyFallingPlatform : MonoBehaviour
{
	public float fallDelay;
	public float lifeTime = 3f;

	private Rigidbody2D _rb2d;
	private Collider2D _col2d;

	private void Start()
	{
		_rb2d = GetComponent<Rigidbody2D>();
		_col2d = GetComponent<Collider2D>();
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.CompareTag("Player"))
		{
			StartCoroutine(Fall());
		}
	}

	private IEnumerator Fall()
	{
		yield return new WaitForSeconds(fallDelay);

		_rb2d.bodyType = RigidbodyType2D.Dynamic;
		_col2d.isTrigger = true;
		yield return new WaitForSeconds(lifeTime);
		Destroy(gameObject);
	}
}