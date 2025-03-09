using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    Debug.LogWarning("[CustomWarning] Object not found thus not created");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError(gameObject);
            Destroy(gameObject);
        }
    }
}
