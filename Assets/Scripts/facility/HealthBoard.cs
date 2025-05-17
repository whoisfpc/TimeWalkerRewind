using UnityEngine;

public class HealthBoard : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			collider.GetComponent<PlayerController>().restoreHealth(50);
			Destroy(gameObject);
		}
	}
}