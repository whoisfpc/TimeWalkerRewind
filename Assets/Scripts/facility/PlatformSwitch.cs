using UnityEngine;
using System.Collections;

public class PlatformSwitch : MonoBehaviour {

	public GameObject hidePlatform;
	public bool hideOnStart = true;
	public bool holdOnMust = false;

	private bool holding = false;

	// Use this for initialization
	void Start () {
		if (hideOnStart) {
			hidePlatform.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (holdOnMust) {
			if (!holding) {
				hidePlatform.SetActive (false);
			} else {
				hidePlatform.SetActive (true);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			hidePlatform.SetActive (true);
		}
	}

	public void OnTriggerStay2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			holding = true;
		}
	}

	public void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			holding = false;
		}
	}
}
