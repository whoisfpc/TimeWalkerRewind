﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MySkillBar : MonoBehaviour {

	public Text timeSlowText;
	public Text timeBombText;
	public Text timeBackText;
	public PlayerController playerCtrl;

	private Color lackEnergyColor = Color.blue;
	private Color canUseColor = Color.white;

	public Image timeSlowIcon;
	public Image timeBombIcon;
	public Image timeBackIcon;

	public Image timeSlowMask;
	public Image timeBombMask;
	public Image timeBackMask;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		int intRemainTime;

		SkillState timeSlowState = playerCtrl.GetSkillState ("TimeSlow");
		SkillState timeBombState = playerCtrl.GetSkillState ("TimeBomb");
		SkillState timeBackState = playerCtrl.GetSkillState ("TimeBack");

		intRemainTime = (int)Mathf.Ceil (timeSlowState.remainCD);
		timeSlowText.text = intRemainTime > 0 ? intRemainTime.ToString () : "";
		timeSlowIcon.color = timeSlowState.lackEnergy ? lackEnergyColor : canUseColor;
		timeSlowMask.fillAmount = 0;

		intRemainTime = (int)Mathf.Ceil (timeBombState.remainCD);
		timeBombText.text = intRemainTime > 0 ? intRemainTime.ToString () : "";
		timeBombIcon.color = timeBombState.lackEnergy ? lackEnergyColor : canUseColor;
		timeBombMask.fillAmount = timeBombState.remainCD / playerCtrl.timeBombCD;

		intRemainTime = (int)Mathf.Ceil (timeBackState.remainCD);
		timeBackText.text = intRemainTime > 0 ? intRemainTime.ToString () : "";
		timeBackIcon.color = timeBackState.lackEnergy ? lackEnergyColor : canUseColor;
		timeBackMask.fillAmount = timeBackState.remainCD / playerCtrl.backCD;
	}
}