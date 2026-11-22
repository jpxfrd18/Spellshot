using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [field: SerializeField] public int manaValue { get; private set; }
    [field: SerializeField] public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }
    public event Action<int> OnDeath;
    public event Action OnHealthChange;
    [SerializeField] private Material grey;

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

    public void ManaDrain()
    {
        if (manaValue == 0)
        {
            return;
        }

        PlayerStats.Instance.IncreaseMana((int)(manaValue * 1.2f));
        manaValue = 0;

        MakeGrayscale(transform);
    }


    private void MakeGrayscale(Transform current)
    {
        if (current == null || current.name == "Health Bar")
        {
            return;
        }

        MeshRenderer mesh = current.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            mesh.material = grey;
        }

        foreach (Transform child in current)
        {
            MakeGrayscale(child);
        }
    }

    private void Die()
    {
        OnDeath?.Invoke(manaValue);
        Destroy(gameObject);
    }
}
