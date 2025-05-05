using UnityEngine;
using System.Collections;

public class TrapChest : MonoBehaviour {

	public GameObject bomb;
	public int bombNum = 5;
	public float bombDelay = 0.3f;
	public float openDelay = 1f;
	private bool available = true;
	private Animator anim;

	private TimeFieldController timefieldController;
	private float curTimeScale;

	private float fallInterval = 0.2f;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		anim.SetFloat ("OpenSpeed", 1 / openDelay);
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		timefieldController = (TimeFieldController)GameObject.Find ("GameController").GetComponent<TimeFieldController> ();
		curTimeScale = timefieldController.getTimescale (transform.position);
		
	}

    public void TriggerTrap()
    {
		if (available) {
			anim.SetTrigger ("ChestOpen");
		}
    }

	public void StartFall()
	{
		StartCoroutine (FallBombs (bombNum));
	}

	private IEnumerator FallBombs(int num) {
		available = false;
		Vector3 position = transform.position;
		Vector3 bp;
		int i = 0;
		while (i < num) {
			bp = position;
			bp.x = bp.x + (Random.value - 0.5f) * 10f;
			GameObject trapBomb =  Instantiate (bomb, bp, Quaternion.identity) as GameObject;
			trapBomb.GetComponent<TrapBomb> ().boomDelay = bombDelay;
			i++;
			//yield return new WaitForSeconds(0.2f);
			for (float timer = fallInterval; timer>0.0f; timer-= Time.deltaTime * curTimeScale){
				yield return 0;
			}
		}
	}
}
