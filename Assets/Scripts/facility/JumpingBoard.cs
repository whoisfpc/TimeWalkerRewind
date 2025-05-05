using UnityEngine;
using System.Collections;

public class JumpingBoard : MonoBehaviour
{
    public float jumpPower = 20000f;
	private bool canjump = true;
	private float timer = 0.0f;
	private float resumeTime = 0.5f;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!canjump) {
			timer += Time.deltaTime;
		}
		if (timer > resumeTime) {
			canjump = true;
			timer = 0;
		}
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
		if (canjump) {
			Rigidbody2D rgbd = collider.GetComponent<Rigidbody2D> ();
			rgbd.linearVelocity = new Vector2 (rgbd.linearVelocity.x, 0);
			rgbd.AddForce (new Vector2 (0, jumpPower));
			canjump = false;
		}
    }
}
