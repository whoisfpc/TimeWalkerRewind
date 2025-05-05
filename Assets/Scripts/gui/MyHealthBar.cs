using UnityEngine;
using System.Collections;
/// <summary>
/// Manages the health bar
/// </summary>
public class MyHealthBar : MonoBehaviour 
{
	
	/// the healthbar's foreground sprite
	public Transform ForegroundSprite;
	
	public GameObject player;
	
	/// <summary>
	/// Initialization, gets the player
	/// </summary>
	void Start()
	{
	}
	
	/// <summary>
	/// Every frame, sets the foreground sprite's width to match the character's health.
	/// </summary>
	public void Update()
	{
		if (player == null)
			return;
		PlayerController playerCtrl = player.GetComponent<PlayerController> ();
		float healthPercent = playerCtrl.getCurHealth () / playerCtrl.maxHealth;
		ForegroundSprite.localScale = new Vector3(healthPercent,1,1);
		
	}
	
}