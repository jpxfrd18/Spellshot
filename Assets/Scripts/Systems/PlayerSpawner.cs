using UnityEngine;

[DefaultExecutionOrder(-50)]
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PauseController pauseController;
    private GameObject playerInstance;
    public Transform playerTransform
    {
        get
        {
            if (RespawnManager.Instance.IsPlayerDead || playerInstance == null)
            {
                return null;
            }

            return playerInstance.transform;
        }
    }

    public static PlayerSpawner Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        if (!playerInstance)
        {
            playerInstance = Instantiate(player, RespawnManager.Instance.playerSpawn.position, Quaternion.identity);

            PlayerRef statsRef = playerInstance.GetComponent<PlayerRef>();
            statsRef.Initialize(playerStats, pauseController);
            statsRef.rotateYaw.Init(RespawnManager.Instance.playerSpawn.rotation.eulerAngles.y);
        }
    }
}
