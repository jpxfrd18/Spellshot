using UnityEngine;

public class EnemyHealthDisplay : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDamageTaken += UpdateHealthDisplay;
        }
    }

    private void UpdateHealthDisplay()
    {
        transform.localScale = new Vector3((float)enemyHealth.currentHealth / enemyHealth.maxHealth * 0.95f, 1f, 1f);
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDamageTaken -= UpdateHealthDisplay;
        }
    }
}
