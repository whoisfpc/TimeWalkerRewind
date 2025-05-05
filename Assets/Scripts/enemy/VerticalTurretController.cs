using UnityEngine;
using System.Collections;

public class VerticalTurretController : MonoBehaviour {

	private GameObject laser;
	private GameObject curLaser;

	public float activateTime = 3.0f;
	public float interval = 3.0f;

	private float timer;
	private bool active = true;

	private TimeFieldController timefieldController;
	private float curTimeScale;

	// Use this for initialization
	void Start () {
		laser = (GameObject)Resources.Load ("effects/Line/Line");
		curLaser = Instantiate (laser, transform.position + transform.up * 5.0f, Quaternion.FromToRotation (Vector3.forward, transform.up))as GameObject;

		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		curTimeScale = timefieldController.getTimescale (transform.position);

		timer += Time.deltaTime * curTimeScale;

		if (active) {
			curLaser.SetActive(true);
			if (timer > activateTime) {
				timer = 0;
				active = false;
			}
		} else {
			curLaser.SetActive(false);
			if (timer > activateTime) {
				timer = 0;
				active = true;
			}
		}


	}
}
