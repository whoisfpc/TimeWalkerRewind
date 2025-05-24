using UnityEngine;

public class EnergyBoard : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			collider.GetComponent<PlayerController>().RestoreEnergy(50);
			Destroy(gameObject);
		}
	}
}