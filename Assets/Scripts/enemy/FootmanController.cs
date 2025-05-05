using UnityEngine;
using System.Collections;

public class FootmanController : MonoBehaviour {

	public float maxSpeed = 30.0f;
	private float tempSpeed = 0.0f;
	public int face = -1;
	private TimeFieldController timefieldController;
	private float curTimeScale;
	private Animator anim;
	public GameObject footmanExplode;
	private Animator footmanAnim;

	private bool startMoving = false;

	private bool quickExplode = false;

	private GameObject leftSide;
	private GameObject rightSide;
	private bool leftSideCollision = false;
	private bool rightSideCollision = false;
	// Use this for initialization
	void Start () {
		anim = transform.GetChild (0).GetComponent<Animator> ();
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
		footmanAnim = transform.Find ("Enemy2").GetComponent<Animator> ();

		// footmanExplode = (GameObject)Resources.Load ("effects/FootmanExplode_01");

		leftSide = transform.Find ("LeftSide").gameObject;
		rightSide = transform.Find ("RightSide").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		curTimeScale = timefieldController.getTimescale (transform.position);

		if (startMoving && GetComponent<Rigidbody2D> ().linearVelocity.magnitude < 0.1f) {
			face *= -1;
		}

		GetComponent<Rigidbody2D> ().linearVelocity = new Vector2(tempSpeed * face,GetComponent<Rigidbody2D> ().linearVelocity.y);
		GetComponent<Rigidbody2D> ().linearVelocity = GetComponent<Rigidbody2D> ().linearVelocity * curTimeScale;
		footmanAnim.speed = curTimeScale;
		getSideCollision ();
		findPlayerNearby ();

		//Debug.Log (leftSideCollision +" "+rightSideCollision);
		if (leftSideCollision) {
			if (!quickExplode) {
				face = 1;
			} else {
				//Debug.Log ("1111");
				explodeItSelf ();
			}
		}
		if (rightSideCollision) {
			if (!quickExplode) {
				face = -1;
			} else {
				explodeItSelf ();
			}
		}


	}

	void FixedUpdate() {
		anim.SetFloat ("Speed",GetComponent<Rigidbody2D> ().linearVelocity.magnitude);
		//Debug.Log (GetComponent<Rigidbody2D> ().velocity.magnitude);
	}

	void findPlayerNearby (){
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (var player in players) {
			Vector3 playerPos = player.transform.position;
			if (Vector3.Distance (playerPos, transform.position)<20.0f) {
				maxSpeed = 0;
				anim.SetBool ("isdead", true);
				StartCoroutine(explode());
			}
		}
	}

	IEnumerator explode() {
		tempSpeed = 0.0f;
		for (float timer = 1.5f;timer>0.0f;timer-= Time.deltaTime * curTimeScale){
			yield return 0;
		}
		//gameObject.GetComponent<BoxCollider2D> ().isTrigger = false;
		Instantiate(footmanExplode,this.transform.position,this.transform.rotation);
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (var player in players) {
			Vector3 playerPos = player.transform.position;
			if (Vector3.Distance (playerPos, transform.position)<30.0f) {
				player.GetComponent<PlayerController> ().takeDamage (40,(playerPos-transform.position).normalized*2000.0f);
			}
		}
		Destroy (this.gameObject);
	}

	private void getSideCollision(){
		leftSideCollision = Physics2D.Linecast(transform.position, leftSide.transform.position, 1 << LayerMask.NameToLayer("Platforms")|1 << LayerMask.NameToLayer("OneWayPlatforms"));
		rightSideCollision = Physics2D.Linecast(transform.position, rightSide.transform.position, 1 << LayerMask.NameToLayer("Platforms")|1 << LayerMask.NameToLayer("OneWayPlatforms"));
	}

	public void explodeItSelf(){
		anim.SetBool ("isdead", true);
		StartCoroutine(explode());
	}

	public void setQuickExplode(int tempface){
		quickExplode = true;
		face *= tempface;
		//maxSpeed *= 2;
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.collider.tag.Equals ("map") && !startMoving) {
			startMoving = true;
			tempSpeed = maxSpeed;
		}
	}

}
