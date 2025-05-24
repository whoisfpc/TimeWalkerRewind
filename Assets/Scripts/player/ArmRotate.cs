using UnityEngine;

public class ArmRotate : MonoBehaviour
{
	public Transform ForwardAxis;
	private PlayerController _player;

	private void Start()
	{
		_player = GetComponentInParent<PlayerController>();
	}

	private void LateUpdate()
	{
		if (MenuManager.Instance.Paused || _player.IsDead)
		{
			return;
		}

		var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var desiredDir = mouseWorldPos - transform.position;
		desiredDir.z = 0;
		desiredDir.Normalize();
		var deltaAngle = Vector3.SignedAngle(ForwardAxis.right, desiredDir, -Vector3.forward);
		if (!_player.FacingRight)
		{
			deltaAngle += 180f;
		}
		var deltaRot = Quaternion.AngleAxis(deltaAngle, -Vector3.forward);
		transform.rotation = deltaRot * transform.rotation;
	}
}