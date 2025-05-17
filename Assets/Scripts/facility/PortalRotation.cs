using UnityEngine;

public class PortalRotation : MonoBehaviour
{
	public float rotateSpeed = 30.0f;

	private void Update()
	{
		transform.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime * -1));
	}
}
