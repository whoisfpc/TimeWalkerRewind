using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public float smoothTimeY, smoothTimeX;
	public GameObject player1;
	public bool bounds;
	public Vector3 minCameraPos;
	public Vector3 maxCameraPos;

	public float cameraHeight;
	public float cameraWidth;
	private Vector2 _velocity;


	// Use this for initialization
	private void Start()
	{
		Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
		Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
		cameraWidth = topRight.x - bottomLeft.x;
		cameraHeight = topRight.y - bottomLeft.y;
	}

	// Update is called once per frame
	private void Update()
	{
		Vector3 target = player1.transform.position;

		float posX = Mathf.SmoothDamp(transform.position.x, target.x, ref _velocity.x, smoothTimeX);
		float posY = Mathf.SmoothDamp(transform.position.y, target.y, ref _velocity.y, smoothTimeY);
		if (bounds)
		{
			transform.position = new Vector3(Mathf.Clamp(posX, minCameraPos.x, maxCameraPos.x),
				Mathf.Clamp(posY, minCameraPos.y, maxCameraPos.y),
				Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
		}
		else
		{
			transform.position = new Vector3(posX, posY, transform.position.z);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(cameraWidth, cameraHeight, 0));
	}

	public void SetMinCameraPos()
	{
		minCameraPos = gameObject.transform.position;
	}

	public void SetMaxCameraPos()
	{
		maxCameraPos = gameObject.transform.position;
	}
}