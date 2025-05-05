using UnityEngine;
using System.Collections;

public class TrapBomb : MonoBehaviour {

	public Sprite preBoomSprite;
	public float boomDelay = 0.3f;
	public float boomArea = 30f;

	private TimeFieldController timefieldController;
	private float curTimeScale;
	private float formerTimeScale;
	private AudioSource preBoomSound;

	// Use this for initialization
	void Start () {
		preBoomSound = GetComponent<AudioSource> ();

		formerTimeScale = 1.0f;
		curTimeScale = 1.0f;
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
		gameObject.GetComponent<Rigidbody2D> ().gravityScale = curTimeScale;
		gameObject.GetComponent<Rigidbody2D> ().linearVelocity = gameObject.GetComponent<Rigidbody2D> ().linearVelocity * curTimeScale / formerTimeScale;
		gameObject.GetComponent<Rigidbody2D> ().angularVelocity = gameObject.GetComponent<Rigidbody2D> ().angularVelocity * curTimeScale / formerTimeScale;

		//gameObject.GetComponent<Rigidbody2D> ().velocity = gameObject.GetComponent<Rigidbody2D> ().velocity * curTimeScale;
		formerTimeScale = curTimeScale;
	}
	
	// Update is called once per frame
	void Update () {
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
		gameObject.GetComponent<Rigidbody2D> ().gravityScale = curTimeScale;
		gameObject.GetComponent<Rigidbody2D> ().linearVelocity = gameObject.GetComponent<Rigidbody2D> ().linearVelocity * curTimeScale / formerTimeScale;
		gameObject.GetComponent<Rigidbody2D> ().angularVelocity = gameObject.GetComponent<Rigidbody2D> ().angularVelocity * curTimeScale / formerTimeScale;
		formerTimeScale = curTimeScale;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
			StartCoroutine (Boom(coll.gameObject));
		}
	}

	private IEnumerator Boom(GameObject player) {
		//TimeWalker.SoundManager.getInstance ().PlaySingle (preBoomSound);
		preBoomSound.Play();
		GetComponent<SpriteRenderer> ().sprite = preBoomSprite;
		for (float timer = boomDelay; timer>0.0f;timer-= Time.deltaTime * curTimeScale){
			yield return 0;
		}
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; i++) {
			if (Vector3.Distance (players[i].transform.position, transform.position) < boomArea) {
				players[i].GetComponent<PlayerController> ().takeDamage (10, Vector3.zero);
			}
		}
		GameObject footmanExplode = (GameObject)Resources.Load ("effects/FootmanExplode_01");
		Instantiate (footmanExplode, transform.position, Quaternion.identity);
		Destroy (this.gameObject);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, boomArea);
	}

}
