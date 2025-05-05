using UnityEngine;
using System.Collections;

public class TimeBombController : MonoBehaviour {

	private float lifeTime = 8.0f;
	private float timer = 0.0f;
	public GameObject TimeBomb;
	// Use this for initialization
	void Start () {
		// TimeBomb = (GameObject)Resources.Load ("Prefebs/TimeBomb");
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > lifeTime) {
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag != "Player") {
			StartCoroutine (timeBombExplode());
		}
	}

	IEnumerator timeBombExplode() {
		GameObject curTimeBomb = Instantiate (TimeBomb, transform.position, Quaternion.identity) as GameObject;
		curTimeBomb.transform.Find ("timefield").gameObject.SetActive(true);
		Destroy (this.gameObject);
		yield return 0;
	}
}
