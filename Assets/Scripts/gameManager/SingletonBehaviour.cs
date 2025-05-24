using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
	public static T Instance { get; private set; }
	
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError($"SingletonBehaviour {typeof(T)} already exists!");
			Destroy(this);
		}
		else
		{
			Instance = this as T;
		}

		AfterAwake();
	}

	private void OnDestroy()
	{
		BeforeDestroy();
		if (Instance == this)
		{
			Instance = null;
		}
	}
	
	protected virtual void AfterAwake() { }
	protected virtual void BeforeDestroy() { }
}
