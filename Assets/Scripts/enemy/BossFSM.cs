using UnityEngine;
using System.Collections;

public class BossFSM : FSM 
{
    public enum FSMState
    {
        None,
        Patrol,
		Chase,
        Phase1,
		Phase2,
		Phase3,
        Dead,
    }

	private Transform playerTran;
	private Rigidbody2D bossRigidbody;

	private TimeFieldController timefieldController;
	private float curTimeScale;
	private Animator anim;

    //Current state that the NPC is reaching
    public FSMState curState;

    //Speed of the tank
	public float chargeForce = 40000.0f;
    private float curSpeed;
	private bool inBattle = false;

    //Bullet
	public float shootInterval = 0.8f;
	public int facingRight = 1;
	public float patrolSpeed = 30.0f;
	public GameObject missile;
	public GameObject footman;
	public float patrolDistance = 100.0f;

    private GameObject enemybullet;
	private GameObject razor;
	private GameObject curRazor;
	private GameObject portal;
	private GameObject missileExplode;
	private Transform eye;
	private Transform shooter;
	private Transform launcherLeft;
	private Transform launcherRight;
	private int maxHealth;
	private int health;
	private GameObject leftSide;
	private GameObject rightSide;
	private bool leftSideCollision = false;
	private bool rightSideCollision = false;
	private bool hitPlayer = false;
	private float curPatrolDistance = 0.0f;
	private Vector3 tempCastPosition;
	private bool inOperation = false;
	private float scaledTime =0.0f;
	private bool operateDead = false;

    //Initialize the Finite state machine for the NPC tank
	protected override void Initialize () 
    {
		curState = FSMState.Patrol;
		bossRigidbody = gameObject.GetComponent<Rigidbody2D> ();
		maxHealth = gameObject.GetComponent<EnemyController>().health;
		health = maxHealth;
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);



		anim = transform.Find ("Enemy2").GetComponent<Animator> ();

		leftSide = transform.Find ("LeftSide").gameObject;
		rightSide = transform.Find ("RightSide").gameObject;

		portal = GameObject.Find ("portal");
		portal.SetActive (false);

		razor = (GameObject)Resources.Load ("effects/Line/Line");
		enemybullet = (GameObject)Resources.Load ("Prefebs/enemybullet");
		missile = (GameObject)Resources.Load ("Prefebs/missile");
		missileExplode = (GameObject)Resources.Load ("effects/FootmanExplode_01");

		eye = transform.Find ("Enemy2").Find ("Body").Find ("eye");
		shooter = transform.Find ("Enemy2").Find ("Body").Find ("shooter");
		launcherLeft = transform.Find ("Enemy2").Find ("Body").Find ("LauncherLeft");
		launcherRight = transform.Find ("Enemy2").Find ("Body").Find ("LauncherRight");

		scaledTime = Time.time;

		gameObject.GetComponent<EnemyController> ().isBoss = true;
	}

    //Update each frame
    protected override void FSMUpdate(){
		playerTran = GameObject.Find ("hero").transform;
		//timefieldController = (timefieldController)GameObject.Find ("GameController").GetComponent<timefieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
		health = gameObject.GetComponent<EnemyController>().health;
		getSideCollision ();

		scaledTime += Time.deltaTime * curTimeScale;

		//health = gameObject.GetComponent<enemyController> ().health;

        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
			case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Phase1: UpdatePhase1State(); break;
			case FSMState.Phase2: UpdatePhase2State(); break;
			case FSMState.Phase3: UpdatePhase3State(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        //Update the time
        elapsedTime += Time.deltaTime;

		if (inBattle) {
			if (Mathf.Abs(playerTran.position.x-transform.position.x) > 120.0f && health > 0) {
				curState = FSMState.Chase;
			} else {
				if (health > (maxHealth * 2 / 3)) {
					curState = FSMState.Phase1;
				} else if (health > (maxHealth / 3)) {
					curState = FSMState.Phase2;
				} else if (health > 0) {
					curState = FSMState.Phase3;
				}
			}
		}

		if (health <= 0) {
			curState = FSMState.Dead;
		}
		//Debug.Log (curState);
    }

    /// <summary>
    /// Patrol state
    /// </summary>
    protected void UpdatePatrolState()
    {
		bossRigidbody.linearVelocity = Vector3.right * patrolSpeed * facingRight;
		//bossRigidbody.AddForce(Vector3.right * 1000.0f * facingRight);
		curPatrolDistance += patrolSpeed * Time.deltaTime;
		if (curPatrolDistance > patrolDistance | hitTheWall()) {
			curPatrolDistance = 0.0f;
			facingRight *= -1;
		}
		if (Mathf.Abs(playerTran.position.x-transform.position.x) < 130.0f) {
			inBattle = true;
			curState = FSMState.Phase1;
		}
	}

	protected void UpdateChaseState(){
		float facing = Mathf.Sign (playerTran.position.x-transform.position.x);
		bossRigidbody.linearVelocity = Vector3.right * patrolSpeed * facing;
	}
		
	protected void UpdatePhase1State(){
		if (!inOperation) {
			if (transform.position.y > playerTran.position.y) {
				if (Mathf.Abs (playerTran.position.x - transform.position.x) > 90.0f) {
					inOperation = true;
					chargeAttack ();
				} else {
					inOperation = true;
					lazorAttack ();
				}
			} else {
				inOperation = true;
				barrageAttack();
			}

		}
    }
		
	protected void UpdatePhase2State(){
		if (!inOperation) {
			if (transform.position.y > playerTran.position.y) {
				if (Mathf.Abs (playerTran.position.x - transform.position.x) > 90.0f) {
					inOperation = true;
					summonAttack ();
				} else {
					inOperation = true;
					lazorAttack ();
				}
			} else {
				if (Random.value < 0.5) {
					inOperation = true;
					barrageAttack ();
				} else {
					inOperation = true;
					ShootAttack ();
				}
			}

		}
	}
		
	protected void UpdatePhase3State(){
		if (!inOperation) {
			if (Random.value < 0.4) {
				inOperation = true;
				missileAttack ();
			} else {
				if (transform.position.y > playerTran.position.y) {
					inOperation = true;
					summonAttack ();
				} else {
					if (Random.value < 0.5) {
						inOperation = true;
						barrageAttack ();
					} else {
						inOperation = true;
						ShootAttack ();
					}
				}
			}
		}
	}
		
    protected void UpdateDeadState(){
		if (!inOperation) {
			if (!operateDead) {
				//Explode ();
				StartCoroutine (bossDead ());
				operateDead = true;
			}
		}
    }

	private void ShootAttack(){
		float facing = Mathf.Sign (playerTran.position.x-transform.position.x);
		StartCoroutine(shoot(facing));
	}

	IEnumerator bossDead() {
		StartCoroutine(explodeItself());
		yield return new WaitForSeconds (3.0f);
		anim.SetBool ("isDead", true);
		yield return 0;
	}

	IEnumerator shoot(float facing) {
		//tracing shoot
		for (int times = 0; times<10 ;times++) {
			Instantiate (enemybullet, shooter.position, Quaternion.FromToRotation (Vector3.right, playerTran.position - shooter.position));
			yield return new WaitForSeconds (0.3f/curTimeScale);
		}

		inOperation = false;
	}

	private void barrageAttack(){
		float facing = Mathf.Sign (playerTran.position.x-transform.position.x);
		StartCoroutine(barrage(facing));
	}

	IEnumerator barrage(float facing) {
		//barrage
		for (int times = 0; times<3 ;times++) {
			for (float radius = 0.0f; radius < 60.0f; radius += 6.0f) {
				Instantiate (enemybullet, shooter.position, Quaternion.FromToRotation (Vector3.right, new Vector3(facing,Mathf.Tan(radius-30.0f))));
			}
			yield return new WaitForSeconds (0.8f/curTimeScale);
		}
		inOperation = false;
	}

	private void chargeAttack(){
		float facing = Mathf.Sign (playerTran.position.x-transform.position.x);
		StartCoroutine(charge(facing));
	}

	IEnumerator charge(float facing) {
		float tempChargeForce = chargeForce;
		anim.SetTrigger ("preCharge");
		yield return new WaitForSeconds (1.0f);//给动画留的时间

		while(!hitTheWall() && !hitPlayer){
			bossRigidbody.AddForce (Vector3.right * tempChargeForce * facing);
			yield return 0;
		}

		while (bossRigidbody.linearVelocity.magnitude > 1.0f) {
			yield return 0;
		}

		for (float timer = 1.5f; timer > 0; timer -= Time.deltaTime) {
			bossRigidbody.linearVelocity = Vector3.right * patrolSpeed * facing * -1;
			yield return 0;
		}
		inOperation = false;

	}

	private void lazorAttack(){
		float facing = Mathf.Sign (playerTran.position.x-transform.position.x);
		StartCoroutine(razorAtk(facing));
	}

	IEnumerator razorAtk(float facing) {
		anim.SetTrigger ("preLazor");
		yield return new WaitForSeconds (1.0f);//给动画留的时间
		curRazor = Instantiate (razor, eye.transform.position, Quaternion.FromToRotation (Vector3.forward, new Vector3(1.0f * facing,-2.0f,0.0f)))as GameObject;
		for (float timer = 3.0f; timer > 0; timer -= Time.deltaTime) {
			curRazor.transform.position = eye.transform.position;
			curRazor.transform.eulerAngles = new Vector3 (curRazor.transform.eulerAngles.x - 15.0f * Time.deltaTime,curRazor.transform.eulerAngles.y,curRazor.transform.eulerAngles.z);
			yield return 0;
		}
		Destroy (curRazor);
		inOperation = false;
	}

	private void missileAttack(){
		StartCoroutine(missileLaunch());
	}

	IEnumerator missileLaunch() {
		anim.SetTrigger ("preMissile");
		yield return new WaitForSeconds (1.0f);
		for (int i = 0; i < 2; i++) {
			Instantiate (missile, new Vector3 (launcherLeft.position.x, launcherLeft.position.y, 0.0f), Quaternion.FromToRotation (Vector3.left, Vector3.up));
			Instantiate (missile, new Vector3 (launcherRight.position.x, launcherRight.position.y, 0.0f), Quaternion.FromToRotation (Vector3.left, Vector3.up));
			yield return new WaitForSeconds (1.0f);
		}
		inOperation = false;
	}

	private void summonAttack(){
		StartCoroutine(summon());
	}

	IEnumerator summon() {
		anim.SetTrigger ("preSummon");
		yield return new WaitForSeconds (1.0f);
		while (bossRigidbody.linearVelocity.magnitude > 1.0f) {
			yield return 0;
		}
		for (int i = 0; i < 2; i++) {
			GameObject rightFootman = Instantiate (footman, transform.position + Vector3.up * 80.0f + Vector3.right * 50.0f, this.transform.rotation)as GameObject;
			GameObject leftFootman = Instantiate (footman, transform.position + Vector3.up * 80.0f + Vector3.left * 50.0f, this.transform.rotation)as GameObject;
			rightFootman.GetComponent<FootmanController> ().setQuickExplode (-1);
			leftFootman.GetComponent<FootmanController> ().setQuickExplode (1);
			yield return new WaitForSeconds(3.0f);
		}
		inOperation = false;
	}

	IEnumerator explodeItself() {
		anim.SetTrigger ("explode");
		for (int i = 15; i > 0; i--) {
			Instantiate (missileExplode,transform.position + new Vector3(Random.Range(-30.0f,30.0f),Random.Range(-30.0f,20.0f),0.0f),this.transform.rotation);
			//Debug.Log (i);
			yield return new WaitForSeconds (0.3f);
		}
		for (int i = 30; i > 0; i--) {
			Instantiate (missileExplode,transform.position + new Vector3(Random.Range(-30.0f,30.0f),Random.Range(-30.0f,25.0f),0.0f),this.transform.rotation);
		}
		Destroy (this.gameObject);
		portal.SetActive (true);
		yield return 0;
	}

	public void takeDamage(int damage){
		health -= damage;
		if (health <= 0) {
			health = 0;
		}
	}

	private void getSideCollision(){
		leftSideCollision = Physics2D.Linecast(transform.position, leftSide.transform.position, 1 << LayerMask.NameToLayer("Platforms")|1 << LayerMask.NameToLayer("OneWayPlatforms"));
		rightSideCollision = Physics2D.Linecast(transform.position, rightSide.transform.position, 1 << LayerMask.NameToLayer("Platforms")|1 << LayerMask.NameToLayer("OneWayPlatforms"));
	}

	private bool hitTheWall(){
		if (leftSideCollision | rightSideCollision) {
			return true;
		} else {
			return false;
		}
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.collider.tag.Equals ("Player")) {
			//playerTran.gameObject.GetComponent<Rigidbody2D> ().AddForce (bossRigidbody.velocity * 200.0f);
			hitPlayer = true;
			other.collider.gameObject.GetComponent<PlayerController> ().takeDamage (10, (other.collider.transform.position - transform.position).normalized * 100.0f);
		}
	}

	void OnCollisionStay2D(Collision2D other){
		if (other.collider.tag.Equals ("Player")) {
			hitPlayer = true;
		}
	}

	void OnCollisionExit2D(Collision2D other){
		if (other.collider.tag.Equals ("Player")) {
			hitPlayer = false;
		}
	}
}
