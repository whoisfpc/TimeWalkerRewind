using System;

using UnityEngine;
using UnityEngine.Serialization;

public class LifeTimeController : MonoBehaviour
{
	[SerializeField]
	[FormerlySerializedAs("lifetime")] 
	private float _lifeTime = 2.0f;

	private void Start()
	{
		Destroy(gameObject, _lifeTime);
	}
}