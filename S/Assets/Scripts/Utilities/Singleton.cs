using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            T result;
            if (!Singleton<T>.applicationIsQuitting)
            {
                object @lock = Singleton<T>._lock;
                lock (@lock)
                {
                    if (Singleton<T>._instance == null)
                    {
                        Singleton<T>._instance = (T)((object)UnityEngine.Object.FindObjectOfType(typeof(T)));
                        if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopening the scene might fix it.");
                            result = Singleton<T>._instance;
                            return result;
                        }
                        if (Singleton<T>._instance == null)
                        {
                            GameObject gameObject = new GameObject();
                            Singleton<T>._instance = gameObject.AddComponent<T>();
                            gameObject.name = "(singleton) " + typeof(T).ToString();
                            UnityEngine.Object.DontDestroyOnLoad(gameObject);
                            Debug.Log(string.Concat(new object[]
                            {
                                "[Singleton] An instance of ",
                                typeof(T),
                                " is needed in the scene, so '",
                                gameObject,
                                "' was created with DontDestroyOnLoad."
                            }));
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " + Singleton<T>._instance.gameObject.name);
                        }
                    }
                    result = Singleton<T>._instance;
                }
                return result;
            }
            Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit. Won't create again - returning null.");
            result = default(T);
            return result;
        }
    }

    public void OnDestroy()
    {
        Singleton<T>.applicationIsQuitting = true;
    }

    private static T _instance;

    private static object _lock = new object();

    private static bool applicationIsQuitting = false;
}
