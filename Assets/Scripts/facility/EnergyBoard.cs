using UnityEngine;

public class EnergyBoard : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D collider)
	{
		Rigidbody2D rgbd = collider.GetComponent<Rigidbody2D>();
		if (rgbd.gameObject.CompareTag("Player"))
		{
			rgbd.gameObject.GetComponent<PlayerController>().restoreEnergy(50);
			Destroy(gameObject);
		}
	}
}