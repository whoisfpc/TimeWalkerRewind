using UnityEngine;

public class MyEnergyBar : MonoBehaviour
{
	public Transform ForegroundBar;
	public GameObject player;

	private PlayerController _playerCtrl;

	private void Start()
	{
		_playerCtrl = player.GetComponent<PlayerController>();
	}

	private void Update()
	{
		if (!_playerCtrl)
		{
			return;
		}

		float energyPercent = _playerCtrl.getCurEnergy() / _playerCtrl.maxEnergy;
		ForegroundBar.localScale = new Vector3(energyPercent, 1, 1);
	}
}