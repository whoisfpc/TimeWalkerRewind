using UnityEngine;
using System.Collections;

public class CameraFellow : MonoBehaviour {


	public float smoothTimeY, smoothTimeX;
	public GameObject player1;
	public GameObject player2;
	public bool bounds;
	public bool twoPlayerMode = false;
	public Vector3 minCameraPos;
	public Vector3 maxCameraPos;

	public float cameraHeight;
	public float cameraWidth;
	private Vector2 velocity;
	private bool needCheckBound;


	// Use this for initialization
	void Start () {
		var bottomLeft = Camera.main.ScreenToWorldPoint (Vector3.zero);
		var topRight = Camera.main.ScreenToWorldPoint (new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
		cameraWidth = topRight.x - bottomLeft.x;
		cameraHeight = topRight.y - bottomLeft.y;
		needCheckBound = twoPlayerMode;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target;
		if (twoPlayerMode) {
			bool player1Dead = player1.GetComponent<PlayerController> ().HasDead();
			bool player2Dead = player2.GetComponent<PlayerController> ().HasDead();
			if (player1Dead) {
				if (player2Dead) {
					target = transform.position;
				} else {
					target = player2.transform.position;
					needCheckBound = false;
				}
			} else {
				if (player2Dead) {
					target = player1.transform.position;
					needCheckBound = false;
				} else {
					target = (player1.transform.position + player2.transform.position) / 2;
				}
			}
		} else {
			target = player1.transform.position;
		}
	
		float posX = Mathf.SmoothDamp (transform.position.x, target.x, ref velocity.x, smoothTimeX);
		float posY = Mathf.SmoothDamp (transform.position.y, target.y, ref velocity.y, smoothTimeY);
		if (bounds) {
			transform.position = new Vector3 (Mathf.Clamp(posX, minCameraPos.x, maxCameraPos.x),
				Mathf.Clamp(posY, minCameraPos.y, maxCameraPos.y),
				Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
		} else {
			transform.position = new Vector3 (posX, posY, transform.position.z);
		}

		CheckBound ();
	}

	public void SetMinCameraPos() {
		minCameraPos = gameObject.transform.position;
	}

	public void SetMaxCameraPos() {
		maxCameraPos = gameObject.transform.position;
	}

	public void CheckBound() {
		if (!needCheckBound) {
			return;
		}
		Vector3 center = (player1.transform.position + player2.transform.position) / 2;
		KeepInBound (center, player1);
		KeepInBound (center, player2);
	}

	private void KeepInBound(Vector3 center, GameObject hero) {
		float x = hero.transform.position.x;
	    float y = hero.transform.position.y;
		if (x > (center.x + cameraWidth / 2)) {
			x = center.x + cameraWidth / 2;
		} else if (x < (center.x - cameraWidth / 2)) {
			x = center.x - cameraWidth / 2;
		}
		if (y > (center.y + cameraHeight / 2)) {
			y = center.y + cameraHeight / 2;
		} else if (y < (center.y - cameraHeight / 2)) {
			y = center.y - cameraHeight / 2;
		}

		hero.transform.position = new Vector3(x, y, hero.transform.position.z);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position, new Vector3(cameraWidth, cameraHeight, 0));
	}

}
