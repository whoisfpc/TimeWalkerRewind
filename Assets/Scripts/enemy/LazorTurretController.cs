using UnityEngine;
using System.Collections;

public class LazorTurretController : MonoBehaviour {

	public GameObject laser;
	private GameObject curLaser;
	public float angle = 90.0f;
	public float rotateSpeed = 20.0f;
	public float iniAngle = 0.0f;
	private float curAngle = 0.0f;
	private int face = 1;

	private TimeFieldController timefieldController;
	private float curTimeScale;

	// Use this for initialization
	void Start () {
		// laser = (GameObject)Resources.Load ("effects/Line/Line");
		curLaser = Instantiate (laser, transform.position, Quaternion.FromToRotation (Vector3.forward, Vector3.down))as GameObject;
		curAngle = iniAngle;

		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		curTimeScale = timefieldController.getTimescale (transform.position);

		//curLaser
		curAngle += rotateSpeed * curTimeScale * Time.deltaTime * face;
		curLaser.transform.Rotate(Vector3.up * rotateSpeed * curTimeScale * face * Time.deltaTime);
		if (curAngle > (angle / 2) || curAngle < (-angle / 2)) {
			face *= -1;
		}
	}
}
