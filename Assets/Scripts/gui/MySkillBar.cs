using UnityEngine;
using UnityEngine.UI;

public class MySkillBar : MonoBehaviour
{
	public Text timeSlowText;
	public Text timeBombText;
	public Text timeBackText;
	public PlayerController playerCtrl;

	public Image timeSlowIcon;
	public Image timeBombIcon;
	public Image timeBackIcon;

	public Image timeSlowMask;
	public Image timeBombMask;
	public Image timeBackMask;
	private readonly Color canUseColor = Color.white;

	private readonly Color lackEnergyColor = Color.blue;
	
	private void Update()
	{
		int intRemainTime;

		SkillState timeSlowState = playerCtrl.GetSkillState("TimeSlow");
		SkillState timeBombState = playerCtrl.GetSkillState("TimeBomb");
		SkillState timeBackState = playerCtrl.GetSkillState("TimeBack");

		intRemainTime = (int)Mathf.Ceil(timeSlowState.RemainCd);
		timeSlowText.text = intRemainTime > 0 ? intRemainTime.ToString() : "";
		timeSlowIcon.color = timeSlowState.LackEnergy ? lackEnergyColor : canUseColor;
		timeSlowMask.fillAmount = 0;

		intRemainTime = (int)Mathf.Ceil(timeBombState.RemainCd);
		timeBombText.text = intRemainTime > 0 ? intRemainTime.ToString() : "";
		timeBombIcon.color = timeBombState.LackEnergy ? lackEnergyColor : canUseColor;
		timeBombMask.fillAmount = timeBombState.RemainCd / playerCtrl.TimeBombCd;

		intRemainTime = (int)Mathf.Ceil(timeBackState.RemainCd);
		timeBackText.text = intRemainTime > 0 ? intRemainTime.ToString() : "";
		timeBackIcon.color = timeBackState.LackEnergy ? lackEnergyColor : canUseColor;
		timeBackMask.fillAmount = timeBackState.RemainCd / playerCtrl.BackCd;
	}
}