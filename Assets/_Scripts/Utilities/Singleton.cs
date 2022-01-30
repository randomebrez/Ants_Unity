using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A static instance instance is similar to a singleton, but instead of destroying any new instances, 
//it overrides the current instance.This is handy for resetting the state and saves you doing manually.
public abstract class StaticInstance<T> : MonoBehaviour
where T : MonoBehaviour
{
	public static T Instance { get; private set; }
	protected virtual void Awake() => Instance = this as T;

	protected virtual void OnApplicationQUit()
	{
		Instance = null;
		Destroy(gameObject);
	}
}


//This transform the static instance into a basic singleton. This will destroy any new version created,
//leaving the original instance intact.
public abstract class Singleton<T> : StaticInstance<T>
where T : MonoBehaviour
{
	protected override void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);
		base.Awake();
	}
}

//Finally we have a persistant version of the singleton. This will survive through scene loads.
//Perfect for system classes which require stateful, persistant data. Or audio sources
//where music plays through loading screens, etc...	
public abstract class PersistantSingleton<T> : Singleton<T>
where T : MonoBehaviour
{
	protected override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(gameObject);
	}
}
