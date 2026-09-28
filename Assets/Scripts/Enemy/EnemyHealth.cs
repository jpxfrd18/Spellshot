using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [field: SerializeField] public int manaValue { get; private set; }
    [field: SerializeField] public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }

    public event Action<int> OnDeath;
    public event Action OnHealthChange;

    public void Awake()
    {
        currentHealth = maxHealth;
    }

    public void DecreaseHealth(int amount)
    {
        if (amount > 0)
        {
            currentHealth -= amount;
            OnHealthChange?.Invoke();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void IncreaseHealth(int amount)
    {
        if (currentHealth == maxHealth || amount < 1)
        {
            return;
        }

        if (currentHealth + amount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }

        OnHealthChange?.Invoke();
    }

    private void Die()
    {
        OnDeath?.Invoke(manaValue);
        Destroy(gameObject);
    }
}
