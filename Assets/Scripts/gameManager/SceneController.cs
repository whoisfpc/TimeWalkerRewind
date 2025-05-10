using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	private static SceneController instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	public static SceneController getInstance()
	{
		if (instance == null)
		{
			GameObject obj = new();
			obj.hideFlags = HideFlags.HideAndDontSave;
			instance = obj.AddComponent<SceneController>();
		}

		return instance;
	}

	public void BackToMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}

	public void Restart()
	{
		StartCoroutine("LoadingScene", SceneManager.GetActiveScene().name);
	}

	public void SinglePlayer(string sceneName)
	{
		StartCoroutine("LoadingScene", sceneName);
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine("LoadingScene", sceneName);
	}

	private IEnumerator LoadingScene(string sceneName)
	{
		AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
		while (!asyncOp.isDone)
		{
			yield return null;
		}
	}
}