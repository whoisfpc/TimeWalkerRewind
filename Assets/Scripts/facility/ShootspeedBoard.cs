using UnityEngine;
using System.Collections;

public class ShootspeedBoard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D collider){
		Rigidbody2D rgbd = collider.GetComponent<Rigidbody2D> ();
		if (rgbd.gameObject.tag == "Player") {
			rgbd.gameObject.GetComponent<ShootController> ().shootSpeedAccelerate (2, 10.0f);
			Destroy (this.gameObject);
		}
	}
}
