using UnityEngine;

public class MyHealthBar : MonoBehaviour
{
	/// the healthBar's foreground sprite
	public Transform ForegroundSprite;
	public GameObject player;
	
	private PlayerController _playerCtrl;

	private void Start()
	{
		_playerCtrl = player.GetComponent<PlayerController>();
	}

	public void Update()
	{
		if (!_playerCtrl)
		{
			return;
		}

		float healthPercent = _playerCtrl.GetCurHealth() / _playerCtrl.MaxHealth;
		ForegroundSprite.localScale = new Vector3(healthPercent, 1, 1);
	}
}