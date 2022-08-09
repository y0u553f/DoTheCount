using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    public virtual void Awake()
    {
        Debug.Log(" I am Awaking" + typeof(T).Name);
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(this);
        }

    }


}
