using UnityEngine;
using System.Collections;

public class TurretShootController : MonoBehaviour {

	private Transform playerTran;
	private GameObject[] players;
	private GameObject enemybullet;
	private TimeFieldController timefieldController;
	public float searchRadius = 120.0f;
	private float curTimeScale;
	private float distance;
	public float shootInterval = 0.8f;
	private float nextShoot = 0.0f;
	private Vector3 predictPosition;
	private Transform headTran;
	public bool tracing = true;
	public Vector3 facingdirection = new Vector3(0.0f,-1.0f,0.0f);
	private float scaledTime =0.0f;
	// Use this for initialization
	void Start () {
		//playerTran = GameObject.FindGameObjectWithTag ("Player").transform;
		players = GameObject.FindGameObjectsWithTag ("Player");
		playerTran = FindPlayerTran ();
		enemybullet = (GameObject)Resources.Load ("Prefebs/enemybullet");
		predictPosition = playerTran.position;
		headTran = transform;
		scaledTime = Time.time;
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		
		curTimeScale = timefieldController.getTimescale (transform.position);
		if (tracing) {
			playerTran = FindPlayerTran ();
			distance = Vector3.Distance (playerTran.position, transform.position);
			//predictPosition = playerTran.position + new Vector3(playerTran.gameObject.GetComponent<Rigidbody2D> ().velocity.x,playerTran.gameObject.GetComponent<Rigidbody2D> ().velocity.y,0) * (playerTran.position - transform.position).magnitude / enemybullet.GetComponent<bulletcontroller> ().speed;
			predictPosition = playerTran.position;
			facingdirection = predictPosition - transform.position;
		}
		scaledTime += Time.deltaTime * curTimeScale;
		if (distance<searchRadius) {
			headTran.rotation = Quaternion.FromToRotation (Vector3.right, facingdirection);
			if (!Physics2D.Linecast(transform.position,playerTran.position, 1 << LayerMask.NameToLayer("Platforms"))){
				if (scaledTime > nextShoot){
					Instantiate (enemybullet, transform.position, Quaternion.FromToRotation (Vector3.right, facingdirection));
					nextShoot = Time.time + shootInterval;
					scaledTime = Time.time;
				}
			}
		}
	}

	private Transform FindPlayerTran(){
		if (players.Length == 1) {
			return players [0].transform;
		} else if (players.Length == 2) {
			if (Vector3.Distance (transform.position, players [0].transform.position) < Vector3.Distance (transform.position, players [1].transform.position)) {
				return players [0].transform;
			} else {
				return players [1].transform;
			}
		} else {
			return null;
		}
	}
}
