using System.Collections;

using UnityEngine;

public class TrapChest : MonoBehaviour
{
	private static readonly int OpenSpeedParamHash = Animator.StringToHash("OpenSpeed");
	private static readonly int ChestOpenParamHash = Animator.StringToHash("ChestOpen");
	private const float FallInterval = 0.2f;
	
	public GameObject bomb;
	public int bombNum = 5;
	public float bombDelay = 0.3f;
	public float openDelay = 1f;

	private Animator _anim;
	private bool _available = true;
	private float _curTimeScale;
	private TimeFieldController _timeFieldController;


	private void Start()
	{
		_anim = GetComponent<Animator>();
		_anim.SetFloat(OpenSpeedParamHash, 1 / openDelay);
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
	}

	private void Update()
	{
		_timeFieldController = GameObject.Find("GameController").GetComponent<TimeFieldController>();
		_curTimeScale = _timeFieldController.getTimescale(transform.position);
	}

	public void TriggerTrap()
	{
		if (_available)
		{
			_anim.SetTrigger(ChestOpenParamHash);
		}
	}

	public void StartFall()
	{
		StartCoroutine(FallBombs(bombNum));
	}

	private IEnumerator FallBombs(int num)
	{
		_available = false;
		Vector3 position = transform.position;
		for (int i = 0; i < num; i++)
		{
			Vector3 bp = position;
			bp.x += ((Random.value - 0.5f) * 10f);
			GameObject trapBomb = Instantiate(bomb, bp, Quaternion.identity);
			trapBomb.GetComponent<TrapBomb>().boomDelay = bombDelay;
			for (float timer = FallInterval; timer > 0.0f; timer -= Time.deltaTime * _curTimeScale)
			{
				yield return 0;
			}
		}
	}
}