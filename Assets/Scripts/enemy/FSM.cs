using UnityEngine;

public class Fsm : MonoBehaviour
{
	private void Start()
	{
		Initialize();
	}

	private void Update()
	{
		FsmUpdate();
	}

	private void FixedUpdate()
	{
		FsmFixedUpdate();
	}

	protected virtual void Initialize() { }
	protected virtual void FsmUpdate() { }
	protected virtual void FsmFixedUpdate() { }
}