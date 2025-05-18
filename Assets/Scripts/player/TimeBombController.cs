using UnityEngine;

public class TimeBombController : MonoBehaviour
{
	public GameObject TimeBomb;

	private const float LifeTime = 8.0f;

	private void Start()
	{
		Destroy(gameObject, LifeTime);
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (!coll.gameObject.CompareTag("Player"))
		{
			// TimeBomb带有“timebomb” tag，用于TimeFieldController统计和控制周围速度
			GameObject curTimeBomb = Instantiate(TimeBomb, transform.position, Quaternion.identity);
			curTimeBomb.transform.Find("timefield").gameObject.SetActive(true);
			Destroy(gameObject);
		}
	}
}