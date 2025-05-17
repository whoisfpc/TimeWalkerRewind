using UnityEngine;

public class ItemFloat : MonoBehaviour
{
	public float floatRange = 2.0f;

	private Vector3 _originPosition;

	// Use this for initialization
	private void Start()
	{
		_originPosition = transform.position;
	}

	// Update is called once per frame
	private void Update()
	{
		transform.position = _originPosition + (Mathf.Sin(Time.time) * floatRange * Vector3.up);
	}
}