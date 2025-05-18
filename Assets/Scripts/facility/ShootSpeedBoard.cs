using UnityEngine;

public class ShootSpeedBoard : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D collider)
	{
		Rigidbody2D rgbd = collider.attachedRigidbody;
		if (rgbd.gameObject.CompareTag("Player"))
		{
			rgbd.gameObject.GetComponent<ShootController>().ShootSpeedAccelerate(2, 10.0f);
			Destroy(gameObject);
		}
	}
}