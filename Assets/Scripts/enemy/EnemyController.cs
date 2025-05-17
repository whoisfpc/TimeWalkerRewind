using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public bool canBeKilled = false;
	public int maxHealth = 100;
	public int health{ get; private set;}
	public int attack = 10;
	public int creatEnergyBubble = 5;
	public GameObject energybubble;
	public bool isBoss = false;
	// Use this for initialization
	void Start () {
		// energybubble = (GameObject)Resources.Load ("Prefebs/energybubble");
		health = maxHealth;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void takeDamage(int damage,Vector3 force, GameObject source){
		if (canBeKilled) {
			health -= damage;
			if (health <= 0) {
				health = 0;
			}
			//gameObject.GetComponent<Rigidbody2D> ().AddForce (force);
			if (!isBoss) {
				if(health == 0){
					for (int i=0;i<creatEnergyBubble;i++){
						var bubble = Instantiate(energybubble,transform.position+new Vector3(Random.Range(-10.0f,10.0f),Random.Range(-10.0f,10.0f),0.0f),Quaternion.FromToRotation(Vector3.right,new Vector3(Random.Range(-1.0f,1.0f),Random.Range(-1.0f,1.0f),0.0f)));
						bubble.GetComponent<EnergyBubbleController>().PlayerTran = source.transform;
					}
					Destroy(this.gameObject);
				}
			}
		}
	}
}
