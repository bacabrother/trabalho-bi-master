using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (!Application.isPlaying) return null;
            if (instance == null)
                instance = FindObjectOfType<T>();
            if (instance == null)
                Debug.Log("Singleton<" + typeof(T) + "> instance has been not found.");
            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<T>();
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }
}

