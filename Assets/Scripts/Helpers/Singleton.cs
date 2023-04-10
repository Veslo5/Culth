using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance;
        public bool IsPersistant;
        
        public virtual void Awake()
        {
            if (IsPersistant)
            {
                if (!Instance)
                {
                    Instance = this as T;
                }
                else
                {
                    Object.Destroy(gameObject);
                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Instance = this as T;
            }
        }
    }