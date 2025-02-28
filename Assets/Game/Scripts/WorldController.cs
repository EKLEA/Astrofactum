using UnityEngine;
using UnityEngine.Splines;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);;
    }
}
