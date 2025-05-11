using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	private static SceneController s_instance;

	private void Awake()
	{
		if (s_instance == null)
		{
			s_instance = this;
			DontDestroyOnLoad(this);
		}
		else if (s_instance != this)
		{
			Destroy(gameObject);
		}
	}

	public static SceneController GetInstance()
	{
		if (s_instance == null)
		{
			GameObject obj = new("SceneController");
			s_instance = obj.AddComponent<SceneController>();
		}

		return s_instance;
	}

	public void BackToMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}

	public void Restart()
	{
		StartCoroutine(LoadingScene(SceneManager.GetActiveScene().name));
	}

	public void SinglePlayer(string sceneName)
	{
		StartCoroutine(LoadingScene(sceneName));
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadingScene(sceneName));
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