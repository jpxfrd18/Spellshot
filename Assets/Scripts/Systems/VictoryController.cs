using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class VictoryController : MonoBehaviour
{
    [SerializeField] private Objective[] objectives;
    private PauseController pauseController;
    private PlayerStats playerStats;
    public static VictoryController Instance { get; private set; }
    private bool winning = false;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        pauseController = PauseController.Instance;
        playerStats = PlayerStats.Instance;
    }

    public void EnemyKilled(string tag)
    {
        foreach (Objective objective in objectives)
        {
            if (objective.EnemyKilled(tag))
            {
                Win();
                break;
            }
        }
    }

    public void PlayerReached(string tag)
    {
        foreach (Objective objective in objectives)
        {
            if (objective.PlayerReached(tag))
            {
                Win();
                break;
            }
        }
    }

    public void Reset()
    {
        foreach (Objective objective in objectives)
        {
            objective.Reset();
        }
    }

    private void Win()
    {
        if (!winning)
        {
            winning = true;
            StartCoroutine(WinRoutine());
        }
    }

    private IEnumerator WinRoutine()
    {
        yield return null;
        yield return null;
        if (!RespawnManager.Instance.IsPlayerDead)
        {
            playerStats.OnWin();
            pauseController.OnWin();
        }
        winning = false;
    }
}