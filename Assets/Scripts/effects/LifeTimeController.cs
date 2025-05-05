using UnityEngine;
using System.Collections;

public class LifeTimeController : MonoBehaviour {

	public float lifetime = 2.0f;
	private float curLifeTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		curLifeTime += Time.deltaTime;
		if (curLifeTime > lifetime) {
			Destroy (this.gameObject);
		}
	}
}
