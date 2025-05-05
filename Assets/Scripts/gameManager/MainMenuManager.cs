using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	public Image loadingImage;
	public string singlePlayerScene = "Beginer";
	public string mutiplePlayerScene = "Beginer";

	private bool loadScene = false;

	public void SinglePlayer() {
		if (!loadScene) {
			loadScene = true;
			loadingImage.gameObject.SetActive (true);
			SceneController.getInstance ().SinglePlayer (singlePlayerScene);
		}
	}

	public void MultiplePlayer() {
		if (!loadScene) {
			loadScene = true;
			loadingImage.gameObject.SetActive (true);
			SceneController.getInstance ().MultiplePlayer (mutiplePlayerScene);
		}
	}

	public void Quit() {
		SceneController.getInstance ().Quit ();
	}
}
