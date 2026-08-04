using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [field: SerializeField] public int manaValue { get; private set; }
    [SerializeField] private int maxHealth;
    private int currentHealth;

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
