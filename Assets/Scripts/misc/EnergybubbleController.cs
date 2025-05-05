using UnityEngine;
using System.Collections;

public class EnergybubbleController : MonoBehaviour {

	private GameObject[] players;
	private float speed = 50.0f;
	private float maxspeed = 80.0f;
	private float accelerate = 30.0f;
	public Transform playerTran;
	private GameObject restoreEffect;
	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag ("Player");
		playerTran = FindPlayerTran();
	}
	
	// Update is called once per frame
	void Update () {
		if (speed < maxspeed) {
			speed += accelerate * Time.deltaTime;
		}
		transform.Translate (-transform.right * speed * Time.deltaTime, Space.World);
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.FromToRotation (Vector3.left, playerTran.position - transform.position), 30.0f * Time.deltaTime);

		if(Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Player"))){
			playerTran.gameObject.GetComponent<PlayerController>().restoreEnergy(5);
			Destroy(this.gameObject);
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
