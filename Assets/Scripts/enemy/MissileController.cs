using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {

	private float speed = 50.0f;
	private float maxspeed = 80.0f;
	private float accelerate = 30.0f;
	private float rotateSpeed = 2.0f;

	private int missileDamage = 10;
	private Transform playerTran;

	private float timer = 0.0f;

	private TimeFieldController timefieldController;
	private float curTimeScale;

	private GameObject missileExplode;

	// Use this for initialization
	void Start () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		int tempIndex = Random.Range (0, players.Length);
		playerTran = players[tempIndex].transform;
		missileExplode = (GameObject)Resources.Load ("effects/FootmanExplode_01");


		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		curTimeScale = timefieldController.getTimescale (transform.position);
		if (speed < maxspeed) {
			speed += accelerate * Time.deltaTime;
		}
		if (timer < 0.5f) {
			transform.Translate (-transform.right * speed * curTimeScale * Time.deltaTime, Space.World);
		} else {
			transform.Translate (-transform.right * speed * curTimeScale * Time.deltaTime, Space.World);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.FromToRotation (Vector3.left, playerTran.position - transform.position), rotateSpeed * curTimeScale * Time.deltaTime);
		}
		if(Physics2D.OverlapCircle(transform.position, 3.0f, 1 << LayerMask.NameToLayer("Player")|1 << LayerMask.NameToLayer("Platforms"))){
			//explode ();
			Destroy (this.gameObject);
		}
	}

	void explode (){
		Instantiate(missileExplode,this.transform.position,this.transform.rotation);
		//Destroy (this.gameObject);

		Collider2D[] hit = Physics2D.OverlapCircleAll (transform.position,10.0f,1 << LayerMask.NameToLayer("Player"));
		for (int i = 0; i < hit.Length; i++) {
			hit[i].GetComponent<Collider2D>().gameObject.GetComponent<PlayerController> ().takeDamage (missileDamage,-transform.right * 100.0f);
		}
	}

	void OnDestroy(){
		explode ();
	}
}
