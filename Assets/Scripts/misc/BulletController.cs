using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float speed = 300.0f;
	private float truespeed;
	private TimeFieldController timefieldController;
	public float lifetime = 5.0f;
	private float life = 0.0f;
	private GameObject bursteffect;

	private Sprite sprite;
	private int width;
	private int height;
	private int ox;
	private int oy;
	public GameObject source; // who shoot the bullet

	// Use this for initialization
	void Start () {
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		truespeed = speed * timefieldController.getTimescale (transform.position);
		bursteffect = (GameObject)Resources.Load ("RocketExplosion");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		truespeed = speed * timefieldController.getTimescale (transform.position);
		transform.Translate (Vector2.right * truespeed *Time.deltaTime);
		Collider2D playerCol;

		life += Time.deltaTime;
		if (life > lifetime) {
			Destroy(this.gameObject);
		}

		if (gameObject.name.Equals ("enemybullet(Clone)")) {
			if(Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Platforms"))){
				Instantiate(bursteffect,transform.position,Quaternion.FromToRotation(Vector3.up,transform.forward));
				Destroy(this.gameObject);
			}
			if (playerCol = Physics2D.OverlapCircle (transform.position, 0.1f, 1 << LayerMask.NameToLayer ("Player"))) {
				playerCol.GetComponent<PlayerController> ().takeDamage (10,transform.right * 5000.0f +Vector3.up * 2000.0f);
				Instantiate(bursteffect,transform.position,Quaternion.FromToRotation(Vector3.up,transform.forward));
				Destroy (this.gameObject);
			}
		}
		if (gameObject.name.Equals ("bullet(Clone)")) {
			if(Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Platforms"))){
				Instantiate(bursteffect,transform.position,Quaternion.FromToRotation(Vector3.up,transform.forward));
				Destroy(this.gameObject);
			}
			if (Physics2D.OverlapCircle (transform.position, 0.1f, 1 << LayerMask.NameToLayer ("Enemies"))) {
				Collider2D[] alltouch = Physics2D.OverlapCircleAll (transform.position, 0.1f,1 << LayerMask.NameToLayer ("Enemies"));
				alltouch[0].gameObject.GetComponent<EnemyController>().takeDamage(10,transform.right * 5000.0f +Vector3.up * 2000.0f, source);
				Instantiate(bursteffect,transform.position,Quaternion.FromToRotation(Vector3.up,transform.forward));
				Destroy (this.gameObject);
			}
		}
	}
}
