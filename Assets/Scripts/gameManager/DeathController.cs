using System.Collections;

using UnityEngine;

public class DeathController : MonoBehaviour
{
	public GameObject ScreenMask;
	public GameObject panel;

	public bool HeroDie { get; private set; }

	private SpriteRenderer _screenMaskSprite;

	private void Start()
	{
		_screenMaskSprite = ScreenMask.GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (HeroDie)
		{
			return;
		}

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		HeroDie = true;
		foreach (GameObject player in players)
		{
			if (!player.GetComponent<PlayerController>().HasDead())
			{
				HeroDie = false;
				break;
			}
		}

		if (HeroDie)
		{
			WakeMask();
		}
	}

	private void WakeMask()
	{
		StartCoroutine(Death());
	}

	private IEnumerator Death()
	{
		var maskColor = _screenMaskSprite.color;
		while (_screenMaskSprite.color.a < 40.0f / 256.0f)
		{
			maskColor.a += (1.0f / 256.0f * 15.0f * Time.deltaTime);
			_screenMaskSprite.color = maskColor;
			yield return null;
		}

		panel.SetActive(true);
	}
}