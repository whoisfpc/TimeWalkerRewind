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

		float energyPercent = _playerCtrl.GetCurEnergy() / _playerCtrl.MaxEnergy;
		ForegroundBar.localScale = new Vector3(energyPercent, 1, 1);
	}
}