using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public bool useJoystick = false;
	public Transform gunTran;
	public GameObject gunLight;
	public GameObject timeField;
	public GameObject TimeBombProto;

	/* input name for controller */
	public string horzionalStr = "Horizontal";
	public string verticalStr = "Vertical";
	public string jumpStr = "Jump";
	public string lightStr = "Light";
	public string timeSlowStr = "TimeSlow";
	public string timeBombStr = "TimeBomb";
	public string timeBackStr = "TimeBack";

	/* player property */
	public float maxHealth = 100;
	public float maxEnergy = 100f;
	public float moveForce = 500f;
	public float maxSpeed = 60f;				// The fastest the player can travel in the x axis.
	public float jumpForce = 12000f;			// Amount of force added when the player jumps.
	public int maxjumpTime = 2;
	[Range(0, 1)]public float backSpeed = 0.7f; // backward walk speed percent
	public float energyRestorePersecond = 2f;
	public float maxRestoreEnergy = 30f;

	/* skill attribute */
	public float timeSlowEnergyPerSecond = 5f;
	public float timeSlowStartUpEnergy = 10f;
	public float backToPastTime = 2f;
	public float backCD = 5f;
	public float backToPastEnergy = 30f;
	public float timeBombEnergy = 33f;
	public float timeBombCD = 10f;

	/* private variable */
	private float curhealth = 100;
	private float curEnergy = 100f;
	private bool canBeDamaged = true;
	private float canBeDamagedTimer = 0f;

	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.

	private int jumpTime = 0;
	private bool jump = false;
	private bool facingRight = true;		// For determining which way the player is currently facing.
	private ArrayList backPositions = new ArrayList(); 
	private GameObject m_Cape;
	private Animator anim;					// Reference to the player's animator component.
	public GameObject arm;
	private bool isPause;

	/* Skill states */
	private SkillState timeSlowState = new SkillState("TimeSlow");
	private SkillState timeBackState = new SkillState("TimeBack");
	private SkillState timeBombState = new SkillState("TimeBomb");

	private bool isdead = false;

	void Awake () {
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = transform.Find ("player").GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
		m_Cape = transform.Find ("cape").gameObject;

		curhealth = maxHealth;
		curEnergy = maxEnergy;
	}

	// Update is called once per frame
	void Update () {
		isPause = GameObject.FindGameObjectWithTag ("GameController").GetComponent<MenuManager> ().paused;

		// test to determine wether player is on ground
		int groundLayerMask = 1 << LayerMask.NameToLayer ("Platforms") | 1 << LayerMask.NameToLayer ("OneWayPlatforms") | 1 << LayerMask.NameToLayer ("Enemies") | 1 << LayerMask.NameToLayer("Player");
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 2 , groundLayerMask);
		grounded = false;
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				grounded = true;
		}


		if (isPause || isdead) {
			return;
		}

		/* all input respone will disable when game is paused */
		if (Input.GetButtonDown (jumpStr)) {
			jump = true;
		}

		if (grounded) {
			jumpTime = maxjumpTime;
		}

		if (!grounded && jumpTime == maxjumpTime) {
			jumpTime--;
		}
		if (Input.GetAxis(verticalStr) < 0) {
			anim.SetBool ("Crouch", true);
		} else {
			if (!Physics2D.Linecast (transform.position, transform.Find ("headCheck").position, 1 << LayerMask.NameToLayer ("Platforms"))) {
				anim.SetBool ("Crouch", false);
			}
		}

		if ((Input.GetAxis(verticalStr) < 0) && Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("OneWayPlatforms"))) {
			Collider2D[] tempOneWayPlatform = Physics2D.OverlapCircleAll(groundCheck.position,10.0f,1 << LayerMask.NameToLayer ("OneWayPlatforms"));
			for (int i = 0; i < tempOneWayPlatform.Length; i++) {
				tempOneWayPlatform[i].transform.gameObject.GetComponent<BoxCollider2D> ().isTrigger = true;
			}
		}

		// Automatic energy recover
		if (curEnergy < maxRestoreEnergy) {
			curEnergy += energyRestorePersecond * Time.deltaTime;
			if (curEnergy > maxRestoreEnergy) {
				curEnergy = maxRestoreEnergy;
			}
		}

		float lookRight;

		if (!useJoystick) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePosition.z = 0.0f;
			Vector3 lookDir = mousePosition - transform.position;
			lookRight = lookDir.x;
		} else {
			lookRight = Input.GetAxis ("Aim_X_P2");
			if (lookRight == 0) {
				lookRight = Input.GetAxis (horzionalStr);
			}

			if (lookRight == 0) {
				lookRight = 1;
			}
		}
		if (isdead) {
			lookRight = 1;
		}
		if (lookRight > 0 && !facingRight) {
			Cloth cl = m_Cape.gameObject.GetComponent<Cloth> ();
			CapsuleCollider[] old_ca = cl.capsuleColliders;
			cl.capsuleColliders = new CapsuleCollider[0];
			Flip ();
			cl.capsuleColliders = old_ca;
		} else if (lookRight < 0 && facingRight) {
			Cloth cl = m_Cape.gameObject.GetComponent<Cloth> ();
			CapsuleCollider[] old_ca = cl.capsuleColliders;
			cl.capsuleColliders = new CapsuleCollider[0];
			Flip ();
			cl.capsuleColliders = old_ca;
		}

		/* Deal with skill input */

		// open or close light
		if (Input.GetButtonDown(lightStr)) {
			gunLight.SetActive (!gunLight.activeSelf);
		}

		// ope or close time slow field
		if (Input.GetButtonDown (timeSlowStr)) {
			SwitchTimeSlow ();
		}
		// Continuous consume energy while time slow field is opening
		if (timeSlowState.onUsing) {
			if (!useEnergy(timeSlowEnergyPerSecond * Time.deltaTime)) {
				timeField.SetActive (false);
				curEnergy = 0;
				timeSlowState.onUsing = false;
			}
		}
		timeSlowState.lackEnergy = curEnergy < timeSlowStartUpEnergy;
		timeSlowState.canUse = !timeSlowState.lackEnergy && (timeSlowState.remainCD == 0);

		// try to shoot time bomb
		timeBombState.remainCD = Mathf.Max (0, timeBombState.remainCD - Time.deltaTime);
		if (Input.GetButtonDown (timeBombStr)) {
			ShootTimeBomb ();
		}
		timeBombState.lackEnergy = curEnergy < timeBombEnergy;
		timeBombState.canUse = !timeBombState.lackEnergy && (timeBombState.remainCD == 0);

		// try to use time back
		timeBackState.remainCD = Mathf.Max (0, timeBackState.remainCD - Time.deltaTime);
		if (Input.GetButtonDown (timeBackStr)) {
			UseTimeBack ();
		}
		timeBackState.lackEnergy = curEnergy < backToPastEnergy;
		timeBackState.canUse = !timeBackState.lackEnergy && (timeBackState.remainCD == 0);
	}

	void FixedUpdate (){
		if (isPause || isdead) {
			return;
		}
		anim.SetBool ("Ground",grounded);
		float h = Input.GetAxis(horzionalStr);
		if (isPause) {
			h = 0;
		}

		bool forward;
		if ((facingRight && h > 0) || (!facingRight && h < 0)) {
			forward = true;
		} else {
			forward = false;
		}
		anim.SetBool ("Forward", forward);
		float temp = 1;
		if (!forward) {
			temp = backSpeed;
		}
		GetComponent<Rigidbody2D> ().linearVelocity = new Vector2(h * temp * maxSpeed,GetComponent<Rigidbody2D> ().linearVelocity.y);
		anim.SetFloat("Speed",Mathf.Abs(GetComponent<Rigidbody2D> ().linearVelocity.x * h));
		if (jumpTime > 0 && jump) {
			GetComponent<Rigidbody2D> ().linearVelocity = new Vector2 (GetComponent<Rigidbody2D> ().linearVelocity.x, 0.0f);
			GetComponent<Rigidbody2D> ().AddForce (Vector2.up * jumpForce);
			jumpTime--;
			jump = false;
		}

		if (backPositions.Count >= backToPastTime * 50) {
			backPositions.RemoveAt(0);
		}
		backPositions.Add (transform.position);
		anim.SetFloat("vSpeed", GetComponent<Rigidbody2D> ().linearVelocity.y/20.0f);
		if (!canBeDamaged) {
			canBeDamagedTimer += Time.deltaTime;
			if (canBeDamagedTimer > 1.0f) {
				canBeDamagedTimer = 0.0f;
				canBeDamaged = true;
			}
		}
	}

	void Flip (){
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void UseTimeBack() {
		if (timeBackState.canUse) {
			if (useEnergy (backToPastEnergy)) {
				transform.Translate ((Vector3)backPositions [0] - transform.position);
				timeBackState.remainCD = backCD;
			}
		}
	}

	void SwitchTimeSlow() {
		if (!timeSlowState.onUsing) {
			if (useEnergy(timeSlowStartUpEnergy)) {
				timeSlowState.onUsing = true;
				timeField.SetActive (true);
			}
		} else {
			timeSlowState.onUsing = false;
			timeField.SetActive (false);
		}
	}


	void ShootTimeBomb() {
		if (timeBombState.canUse) {
			if (useEnergy (timeBombEnergy)) {
				if (!useJoystick) {
					Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					mousePosition.z = 0.0f;
					GameObject curBomb = Instantiate (TimeBombProto, gunTran.position, Quaternion.FromToRotation (Vector3.right, mousePosition - gunTran.position)) as GameObject;
					curBomb.GetComponent<Rigidbody2D> ().AddForce ((mousePosition - gunTran.position).normalized * 5000.0f);
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
					GameObject curBomb = Instantiate (TimeBombProto, gunTran.position, Quaternion.identity) as GameObject;
					curBomb.GetComponent<Rigidbody2D> ().AddForce ((new Vector2 (x, y)).normalized * 5000.0f);
				}
				timeBombState.remainCD = timeBombCD;
			}
		}
	}

	public Vector3 getVelocity(){
		return GetComponent<Rigidbody2D> ().linearVelocity;
	}

	public float getCurHealth(){
		return curhealth;
	}

	public float getCurEnergy(){
		return curEnergy;
	}

	public void takeDamage(int damage, Vector3 force){
		if (!isdead) {
			if (canBeDamaged) {
				curhealth -= damage;
				gameObject.GetComponent<Rigidbody2D> ().AddForce (force);
				canBeDamaged = false;
			}
			if (curhealth <= 0) {
				isdead = true;
				if (timeSlowState.onUsing) {
					SwitchTimeSlow ();
				}
				arm.SetActive (false);
				GameObject armR = transform.Find ("player").Find ("spine").Find ("armR").gameObject;
				GameObject armL = transform.Find ("player").Find ("spine").Find ("armL").gameObject;
				armR.SetActive (true);
				armL.SetActive (true);
				transform.Find ("cape").gameObject.SetActive (false);
				anim.SetTrigger ("isdead");
				curhealth = 0;
				//GameObject.FindGameObjectWithTag ("GameController").GetComponent<deathController> ().WakeMask ();
			}
		}
	}

	public void restoreHealth(int restore){
		curhealth += restore;
		if (curhealth > maxHealth) {
			curhealth = maxHealth;
		}
	}

	public void restoreEnergy(int restore){
		curEnergy += restore;
		if (curEnergy > maxEnergy) {
			curEnergy = maxEnergy;
		}
	}

	public bool getFacingRight(){
		return facingRight;
	}

	public bool useEnergy(float energy){
		if (curEnergy >= energy) {
			curEnergy -= energy;
			return true;
		} else {
			return false;
		}
	}

	public SkillState GetSkillState(string skillName) {
		switch (skillName) {
			case "TimeSlow":
				return timeSlowState;
			case "TimeBack":
				return timeBackState;
			case "TimeBomb":
				return timeBombState;
			default:
				return null;
		}
	}

	public bool HasDead() {
		return isdead;
	}
}

public class SkillState {
	public string skillName;
	public float remainCD;
	public bool onUsing;
	public bool canUse;
	public bool lackEnergy;

	public SkillState(string skillName) {
		this.skillName = skillName;
		this.remainCD = 0;
		this.onUsing = false;
		this.canUse = true;
		this.lackEnergy = false;
	}
}