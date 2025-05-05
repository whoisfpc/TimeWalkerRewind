using UnityEngine;
using System.Collections;

public class ItemFloat : MonoBehaviour {

	public float floatRange = 2.0f;
	public Vector3 originPosition;
	// Use this for initialization
	void Start () {
		originPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = originPosition + Mathf.Sin (Time.time) * floatRange * Vector3.up;
	}
}
