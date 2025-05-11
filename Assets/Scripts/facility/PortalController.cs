using UnityEngine;
using System.Collections;

public class PortalController : MonoBehaviour {

	public string scenename;
	public bool backToMenu = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.tag.Equals ("Player")) {
			if (backToMenu) {
				SceneController.GetInstance ().BackToMenu ();
			} else {
				SceneController.GetInstance ().LoadScene (scenename);
			}
		}
	}
}
