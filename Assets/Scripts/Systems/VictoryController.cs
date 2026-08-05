using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class VictoryController : MonoBehaviour
{
    [SerializeField] private Objective[] objectives;
    private PauseController pauseController;
    private PlayerStats playerStats;
    public static VictoryController Instance { get; private set; }
    public event Action<string> OnUpdateObjective;
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

        OnUpdateObjective?.Invoke(ToString());
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
        OnUpdateObjective?.Invoke(ToString());
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

    public override string ToString()
    {
        if (objectives.Length == 1)
        {
            return objectives[0].ToString();
        }

        string result = "";
        foreach (Objective objective in objectives)
        {
            result += objective.ToString() + " OR ";
        }
        //Remove trailing " OR "
        return result.Substring(0, result.Length - 4);
    }
}