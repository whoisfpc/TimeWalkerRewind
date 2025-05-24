using System.Collections.Generic;

using UnityEngine;

public class TimeFieldController : MonoBehaviour
{
	public float fieldwidth = 150.0f;

	private readonly List<Vector3> _timePoints = new();

	private void Start()
	{
		getTimepoints();
	}

	private void Update()
	{
		getTimepoints();
	}

	private void getTimepoints()
	{
		_timePoints.Clear();
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < players.Length; i++)
		{
			SkillState timeSlowState = players[i].GetComponent<PlayerController>().GetSkillState("TimeSlow");
			if (timeSlowState.OnUsing)
			{
				_timePoints.Add(players[i].transform.position);
			}
		}

		GameObject[] timebombs = GameObject.FindGameObjectsWithTag("timebomb");
		for (int i = 0; i < timebombs.Length; i++)
		{
			_timePoints.Add(timebombs[i].transform.position);
		}
	}

	public float getTimescale(Vector3 point)
	{
		float timescale = 1.0f;
		foreach (var t in _timePoints)
		{
			float curdistance = Vector3.Distance(point, (Vector3)t);
			if (curdistance < fieldwidth)
			{
				float curscale = 0.1f + (curdistance / fieldwidth * (curdistance / fieldwidth) * 0.3f);
				if (curscale < timescale)
				{
					timescale = curscale;
				}
			}
		}

		return timescale;
	}
}