using UnityEngine;

public class PlatformSwitch : MonoBehaviour
{
	public GameObject hidePlatform;
	public bool hideOnStart = true;
	public bool holdOnMust;

	private bool _holding;

	private void Start()
	{
		if (hideOnStart)
		{
			hidePlatform.SetActive(false);
		}
	}

	private void Update()
	{
		if (holdOnMust)
		{
			if (!_holding)
			{
				hidePlatform.SetActive(false);
			}
			else
			{
				hidePlatform.SetActive(true);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			hidePlatform.SetActive(true);
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			_holding = false;
		}
	}

	public void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			_holding = true;
		}
	}
}