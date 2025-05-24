using UnityEngine;

public class MenuManager : SingletonBehaviour<MenuManager>
{
	public GameObject pauseUI;
	public bool Paused { get; private set; }

	// Use this for initialization
	private void Start()
	{
		pauseUI.SetActive(false);
		Paused = false;
	}

	// Update is called once per frame
	private void Update()
	{
		if (GetComponent<DeathController>().HeroDie)
		{
			return;
		}

		if (Input.GetButtonDown("Pause"))
		{
			Paused = !Paused;
		}

		if (Paused)
		{
			pauseUI.SetActive(true);
			Time.timeScale = 0;
		}
		else
		{
			pauseUI.SetActive(false);
			Time.timeScale = 1;
		}
	}

	public void Resume()
	{
		Paused = false;
	}

	public void Pause()
	{
		Paused = true;
	}

	public void BackToMenu()
	{
		SceneController.GetInstance().BackToMenu();
	}

	public void Restart()
	{
		SceneController.GetInstance().Restart();
	}

	public void Quit()
	{
		SceneController.GetInstance().Quit();
	}
}