using UnityEngine;
using System.Collections;

public class MyEnergyBar : MonoBehaviour {

	public Transform ForegroundBar;
	
	public  GameObject player;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null)
			return;
		PlayerController playerCtrl = player.GetComponent<PlayerController> ();
		float energyPercent = playerCtrl.getCurEnergy() / playerCtrl.maxEnergy;
		ForegroundBar.localScale = new Vector3(energyPercent, 1, 1);
	}
}
