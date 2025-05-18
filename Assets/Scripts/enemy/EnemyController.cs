using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public int maxHealth = 100;
	public int creatEnergyBubble = 5;
	public bool isBoss;
	[SerializeField]
	private EnergyBubbleController _energyBubblePrefab;

	public int Health { get; private set; }

	private void Start()
	{
		Health = maxHealth;
	}

	public void TakeDamage(int damage, Vector3 force, GameObject source)
	{

		Health = Mathf.Max(0, Health - damage);

		if (isBoss || Health != 0)
		{
			return;
		}

		for (int i = 0; i < creatEnergyBubble; i++)
		{
			var bubblePos = transform.position
			                + new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0.0f);
			var bubbleRot = Quaternion.FromToRotation(Vector3.right,
				new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f));
			var bubbleCtrl = Instantiate(_energyBubblePrefab, bubblePos, bubbleRot);
			bubbleCtrl.PlayerTran = source.transform;
		}

		Destroy(gameObject);
	}
}