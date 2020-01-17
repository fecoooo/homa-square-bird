using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T>:MonoBehaviour where T : MonoBehaviourSingleton<T>
{
	public static T instance;

	protected void Awake()
	{
		if (instance != null)
		{
			Debug.LogError($"Multiple instances of singleton class: {typeof(T)}\n");
			Debug.Break();
		}
		instance = (T)this;

		OnAwake();
	}

	protected virtual void OnAwake()
	{ }

	protected void OnDestroy()
	{
		instance = null;
	}
}