using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private PlayerStats playerStats;
    private VictoryController victoryController;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        playerStats = PlayerStats.Instance;
        victoryController = VictoryController.Instance;
    }

    private void OnEnable()
    {
        enemyHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        enemyHealth.OnDeath -= HandleDeath;
    }

    private void HandleDeath(int manaValue)
    {
        playerStats.IncreaseMana(manaValue);
        victoryController.EnemyKilled(gameObject.tag);
    }
}
