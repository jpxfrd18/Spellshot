using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private PlayerStats playerStats;
    private VictoryController victoryController;
    [SerializeField] private GameObject deathEffectPrefab;

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


        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        }
    }
}
