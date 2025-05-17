using UnityEngine;

public class PortalController : MonoBehaviour
{
	public string scenename;

	public bool backToMenu;

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			if (backToMenu)
			{
				SceneController.GetInstance().BackToMenu();
			}
			else
			{
				SceneController.GetInstance().LoadScene(scenename);
			}
		}
	}
}