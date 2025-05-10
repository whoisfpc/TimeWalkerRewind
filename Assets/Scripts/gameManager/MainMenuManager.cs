using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public Image loadingImage;
	public string singlePlayerScene = "Beginer";
	bool _loadScene = false;

	public void SinglePlayer()
	{
		if (!_loadScene)
		{
			_loadScene = true;
			loadingImage.gameObject.SetActive(true);
			SceneController.getInstance().SinglePlayer(singlePlayerScene);
		}
	}

	public void Quit()
	{
		SceneController.getInstance().Quit();
	}
}