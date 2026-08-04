using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class RespawnManager : MonoBehaviour
{

    public static RespawnManager Instance { get; private set; }
    [field: SerializeField] public Transform playerSpawn { get; private set; }
    public bool IsPlayerDead { get; private set; } = false;
    private VictoryController victoryController;
    private Vector3 originalSpawnPosition;

    private void Awake()
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

        originalSpawnPosition = playerSpawn.position;
    }

    private void Start()
    {
        victoryController = VictoryController.Instance;
    }

    public void Die()
    {
        if (!IsPlayerDead)
        {
            IsPlayerDead = true;
            victoryController.Reset();
            StartCoroutine(RespawnRoutine());
        }
    }

    public void Restart()
    {
        playerSpawn.position = originalSpawnPosition;
        Die();
    }

    private IEnumerator RespawnRoutine()
    {
        yield return null;
        IsPlayerDead = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
