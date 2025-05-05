using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {

	private int gameMode;
	private static SceneController instance = null;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}

	public static SceneController getInstance() {
		if (instance == null) {
			GameObject obj = new GameObject ();
			obj.hideFlags = HideFlags.HideAndDontSave;
			instance = obj.AddComponent<SceneController> ();
		}
		return instance;
	}

	public void BackToMenu() {
		SceneManager.LoadScene ("Main Menu");
	}

	public void Restart() {
		StartCoroutine ("LoadingScene", SceneManager.GetActiveScene().name);
	}

	public void SinglePlayer(string sceneName) {
		gameMode = 1;
		StartCoroutine ("LoadingScene", sceneName);
	}

	public void MultiplePlayer(string sceneName) {
		gameMode = 2;
		StartCoroutine ("LoadingScene", sceneName);
	}

	public void Quit() {
		Application.Quit ();
	}

	public void LoadScene(string sceneName) {
		StartCoroutine ("LoadingScene", sceneName);
	}
		
	void ApplyConfig() {
		GameObject camera = GameObject.Find ("Main Camera");
		GameObject hero2 = GameObject.Find ("hero 2");
		GameObject hud2 = GameObject.Find ("HUD 2");
		if (gameMode == 1) {
			Debug.Log ("mode 1");
			camera.GetComponent<CameraFellow> ().twoPlayerMode = false;
			hero2.SetActive (false);
			hud2.SetActive (false);
		} else if (gameMode == 2) {
			Debug.Log ("mode 2");
			camera.GetComponent<CameraFellow> ().twoPlayerMode = true;
			hero2.SetActive (true);
			hud2.SetActive (true);
		} else {
			Debug.Log ("Scene Controller: error gamemode = " + gameMode);
		}
	}

	IEnumerator LoadingScene(string sceneName) {
		AsyncOperation asyncOp = SceneManager.LoadSceneAsync (sceneName);
		while (!asyncOp.isDone) {
			yield return null;
		}
		ApplyConfig ();
	}
}