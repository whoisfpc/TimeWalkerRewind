using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
	/// the healthBar's foreground sprite
	public Transform ForegroundSprite;
	public GameObject boss;

	private EnemyController _enemyCtrl;

	private void Start()
	{
		_enemyCtrl = boss.GetComponent<EnemyController>();
	}

	public void Update()
	{
		if (!_enemyCtrl)
		{
			return;
		}

		float healthPercent = _enemyCtrl.health / (float)_enemyCtrl.maxHealth;
		ForegroundSprite.localScale = new Vector3(healthPercent, 1, 1);
	}
}