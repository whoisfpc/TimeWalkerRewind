using UnityEngine;
using System.Collections;

public class PortalRotation : MonoBehaviour {

	public float rotateSpeed = 30.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.forward * rotateSpeed * Time.deltaTime * -1);
	}
}
