using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static SpawnPoint Instance { get; private set; }

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
