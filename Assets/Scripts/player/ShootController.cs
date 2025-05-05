using UnityEngine;
using System.Collections;

public class ShootController : MonoBehaviour {

	public bool useJoystick = false;

	public string fireStr = "Fire";
	public GameObject bullet;
	public GameObject gun;
	private Transform gunTran;
	private Camera MainCamera;

	public float fireRate = 0.2f;
	private float nextfire = 0.0f;

	// Use this for initialization
	void Start () {
		//MainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (!MainCamera){
			MainCamera = Camera.main;
		}

	}
	
	// Update is called once per frame
	void Update () {
		bool isPause = GameObject.FindGameObjectWithTag ("GameController").GetComponent<MenuManager> ().paused;
		bool isDead = GetComponent<PlayerController> ().HasDead ();
		if (!isPause && !isDead) {
			if (Time.time > nextfire) {
				if (Input.GetButton(fireStr)) {
					if (!useJoystick) {
						Vector3 mousePosition = MainCamera.ScreenToWorldPoint (Input.mousePosition);
						mousePosition.z = 0.0f;
						gunTran = gun.transform;
						var bulletInst = Instantiate (bullet, gunTran.position, Quaternion.FromToRotation (Vector3.right, mousePosition - gunTran.position));
						bulletInst.GetComponent<BulletController> ().source = gameObject;
					} else {
						float y = Input.GetAxis ("Aim_Y_P2");
						float x = Input.GetAxis ("Aim_X_P2");
						if (x == 0 && y == 0) {
							y = Input.GetAxis ("Vertical_P2");
							x = Input.GetAxis ("Horizontal_P2");
						}
						if (x == 0 && y == 0) {
							y = 0;
							x = 1;
						}
						float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
						gunTran = gun.transform;
						var bulletInst = Instantiate (bullet, gunTran.position, Quaternion.Euler (new Vector3 (0, 0, angle)));
						bulletInst.GetComponent<BulletController> ().source = gameObject;
					}
					nextfire = Time.time + fireRate;

				}
			}
		}
	}


	public void shootSpeedAccelerate(int scale ,float duration) {
		StartCoroutine(shootSpeedAcc(scale,duration));
	}

	IEnumerator shootSpeedAcc(int scale ,float duration){
		fireRate /= scale;
		yield return new WaitForSeconds (duration);
		fireRate *= scale;
	}
}
