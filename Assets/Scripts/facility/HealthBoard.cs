using UnityEngine;
using System.Collections;

public class HealthBoard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D collider){
		Rigidbody2D rgbd = collider.GetComponent<Rigidbody2D> ();
		if (rgbd.gameObject.tag == "Player") {
			rgbd.gameObject.GetComponent<PlayerController> ().restoreHealth (50);
			Destroy (this.gameObject);
		}
	}
}
