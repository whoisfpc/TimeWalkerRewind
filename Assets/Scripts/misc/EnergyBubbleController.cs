using System;

using UnityEngine;

public class EnergyBubbleController : MonoBehaviour
{
	private const float Accelerate = 30.0f;
	private const float MaxSpeed = 80.0f;
	public Transform PlayerTran { get; set; }

	private GameObject[] _players;
	private float _speed = 50.0f;

	// Use this for initialization
	private void Start()
	{
		_players = GameObject.FindGameObjectsWithTag("Player");
		PlayerTran = FindPlayerTran();
	}

	// Update is called once per frame
	private void Update()
	{
		if (_speed < MaxSpeed)
		{
			_speed += Accelerate * Time.deltaTime;
		}

		transform.Translate(-transform.right * (_speed * Time.deltaTime), Space.World);
		transform.rotation = Quaternion.Slerp(transform.rotation,
			Quaternion.FromToRotation(Vector3.left, PlayerTran.position - transform.position), 30.0f * Time.deltaTime);

		if (Physics2D.OverlapCircle(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Player")))
		{
			PlayerTran.gameObject.GetComponent<PlayerController>().RestoreEnergy(5);
			Destroy(gameObject);
		}
	}

	private Transform FindPlayerTran()
	{
		if (_players.Length == 1)
		{
			return _players[0].transform;
		}

		if (_players.Length == 2)
		{
			if (Vector3.Distance(transform.position, _players[0].transform.position) <
			    Vector3.Distance(transform.position, _players[1].transform.position))
			{
				return _players[0].transform;
			}

			return _players[1].transform;
		}

		return null;
	}
}