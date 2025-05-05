using UnityEngine;
using System.Collections;

public class ArmRotate : MonoBehaviour {
	public bool joystick2Aim = false;

	public float offset = 0f;
	public GameObject spine;
	public GameObject gunlight;
	private PlayerController playercontroller;
	private bool facingRight;

	public float lastAngle = 0f;
	// Use this for initialization
	void Start () {
		if (spine == null) {
			spine = GameObject.Find ("/Player/spine");
		}
		playercontroller = (PlayerController)GameObject.Find ("hero").GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
		bool isPause = GameObject.FindGameObjectWithTag ("GameController").GetComponent<MenuManager> ().paused;
		if (isPause) {
			return;
		}
		if (facingRight != playercontroller.getFacingRight ()) {
			facingRight = playercontroller.getFacingRight ();
		}
		if (!joystick2Aim) {
			Vector3 mouse_pos = Input.mousePosition;
			Vector3 player_pos = Camera.main.WorldToScreenPoint (this.transform.position);
			mouse_pos.x = mouse_pos.x - player_pos.x;
			mouse_pos.y = mouse_pos.y - player_pos.y;

			float angle = Mathf.Atan2 (mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg + offset;
			if (mouse_pos.x > 0) {
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else {
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180 - angle));
			}
			Vector3 raw_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			raw_pos.z = gunlight.transform.position.z;
			raw_pos = raw_pos + mouse_pos*10f;
			gunlight.transform.LookAt (raw_pos);

		} else {
			float y = Input.GetAxis ("Aim_Y_P2");
			float x = Input.GetAxis ("Aim_X_P2");
			if (Mathf.Abs (x) < 0.02 && Mathf.Abs (y) < 0.02) {
				y = Input.GetAxis ("Vertical_P2");
				x = Input.GetAxis ("Horizontal_P2");
			}
			if (Mathf.Abs (x) < 0.02 && Mathf.Abs (y) < 0.02) {
				y = 0;
				x = 1;
			}
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg + offset;
			if (x > 0) {
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else {
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180 - angle));
			}
			gunlight.transform.LookAt (gunlight.transform.position + new Vector3(10 * x, 10 * y , 0));
		}
		this.transform.localPosition = new Vector3 (spine.transform.localPosition.x, spine.transform.localPosition.y, this.transform.localPosition.z);

	}
}
