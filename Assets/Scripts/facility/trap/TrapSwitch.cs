using UnityEngine;

public class TrapSwitch : MonoBehaviour
{
	[SerializeField]
	private TrapChest _trapChestCtrl;

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			_trapChestCtrl.TriggerTrap();
		}
	}
}
