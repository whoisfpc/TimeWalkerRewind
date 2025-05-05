using UnityEngine;
using System.Collections;

public class DeathController : MonoBehaviour {

	public GameObject ScreenMask;
	public GameObject panel;

	public bool heroDie{ get; private set;}
	void Update() {
		if (heroDie) {
			return;
		}
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		heroDie = true;
		foreach (var player in players) {
			if (!player.GetComponent<PlayerController> ().HasDead ()) {
				heroDie = false;
				break;
			}
		}
		if (heroDie) {
			WakeMask ();
		}
	}

	public void WakeMask() {
		StartCoroutine(Death()); 
	}

	IEnumerator Death() {
		while (ScreenMask.GetComponent<SpriteRenderer> ().color.a < 40.0f/256.0f) {
			ScreenMask.GetComponent<SpriteRenderer> ().color = new Color (ScreenMask.GetComponent<SpriteRenderer> ().color.r, ScreenMask.GetComponent<SpriteRenderer> ().color.g, ScreenMask.GetComponent<SpriteRenderer> ().color.b, ScreenMask.GetComponent<SpriteRenderer> ().color.a + 1.0f/256.0f * 15.0f * Time.deltaTime);
			yield return 0;
		}
		panel.SetActive (true);
	}
}
