using UnityEngine;

public class DamageOnStart : MonoBehaviour
{
    [SerializeField] private int damage;

    private void Start()
    {
        EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.DecreaseHealth(damage);
        }
    }
}
