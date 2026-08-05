using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [field: SerializeField] public int manaValue { get; private set; }
    [field: SerializeField] public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }

    public event Action<int> OnDeath;
    public event Action OnDamageTaken;

    public void Awake()
    {
        currentHealth = maxHealth;
    }

    public void DecreaseHealth(int amount)
    {
        if (amount > 0)
        {
            currentHealth -= amount;
            OnDamageTaken?.Invoke();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke(manaValue);
        Destroy(gameObject);
    }
}
