using UnityEngine;
using System.Collections;
public class BossHealthBar : MonoBehaviour 
{

	/// the healthbar's foreground sprite
	public Transform ForegroundSprite;

	public GameObject boss;

	void Start()
	{
	}

	public void Update()
	{
		if (boss == null)
			return;
		EnemyController enemyCtrl = boss.GetComponent<EnemyController> ();
		float healthPercent = (float)enemyCtrl.health / (float)enemyCtrl.maxHealth;
		ForegroundSprite.localScale = new Vector3(healthPercent,1,1);

	}

}