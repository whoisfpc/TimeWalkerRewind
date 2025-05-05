using UnityEngine;
using System.Collections;

public class TimeFieldController : MonoBehaviour {
	
	private ArrayList timePoints = new ArrayList(); 
	public float fieldwidth = 150.0f;
	// Use this for initialization

	void getTimepoints(){
		timePoints.Clear();
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		for (int i=0; i<players.Length; i++) {
			SkillState timeSlowState = players [i].GetComponent<PlayerController> ().GetSkillState ("TimeSlow");
			if (timeSlowState.onUsing){
				timePoints.Add(players[i].transform.position);
			}
		}
		GameObject[] timebombs = GameObject.FindGameObjectsWithTag("timebomb");
		for (int i=0; i<timebombs.Length; i++) {
			timePoints.Add(timebombs[i].transform.position);
		}
	}

	void Start () {
		//timePoints = new ArrayList();
		getTimepoints();
		//Debug.Log (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		getTimepoints();
	}

	public float getTimescale(Vector3 point){
		float timescale = 1.0f;
		for (int i=0; i<timePoints.Count; i++) {
			float curdistance = Vector3.Distance(point,(Vector3)timePoints[i]);
			if (curdistance<fieldwidth){
				//float curscale = 0.1f+(curdistance/fieldwidth)*(curdistance/fieldwidth)*0.9f;
				float curscale = 0.1f + (curdistance/fieldwidth)*(curdistance/fieldwidth)*0.3f;
				//float curscale = 0.1f;
				if(curscale<timescale){
					timescale=curscale;
				}
			}
		}
		return timescale;
	}
}
