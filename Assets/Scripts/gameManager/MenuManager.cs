using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	public GameObject pauseUI;
	public Image loadingImage;
	public bool paused{ get; private set; }

	// Use this for initialization
	void Start () {
		pauseUI.SetActive (false);
		paused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<DeathController>().HeroDie) {
			return;
		}
		if (Input.GetButtonDown ("Pause")) {
			paused = !paused;
		}
		if (paused) {
			pauseUI.SetActive (true);
			Time.timeScale = 0;
		} else {
			pauseUI.SetActive (false);
			Time.timeScale = 1;
		}
	}

	public void Resume() {
		paused = false;
	}

	public void Pause() {
		paused = true;
	}

	public void BackToMenu() {
		SceneController.GetInstance ().BackToMenu ();
	}

	public void Restart() {
		SceneController.GetInstance ().Restart ();
	}

	public void Quit() {
		SceneController.GetInstance ().Quit ();
	}
}
