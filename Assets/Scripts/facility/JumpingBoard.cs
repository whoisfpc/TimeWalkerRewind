using UnityEngine;

public class JumpingBoard : MonoBehaviour
{
	public float jumpPower = 20000f;
	private bool _canJump = true;
	private readonly float _resumeTime = 0.5f;
	private float _timer;

	// Update is called once per frame
	private void FixedUpdate()
	{
		if (!_canJump)
		{
			_timer += Time.deltaTime;
		}

		if (_timer > _resumeTime)
		{
			_canJump = true;
			_timer = 0;
		}
	}

	public void OnTriggerEnter2D(Collider2D collider)
	{
		if (!_canJump)
		{
			return;
		}

		Rigidbody2D rgbd = collider.attachedRigidbody;
		rgbd.linearVelocity = new Vector2(rgbd.linearVelocity.x, 0);
		rgbd.AddForce(new Vector2(0, jumpPower));
		_canJump = false;
	}
}