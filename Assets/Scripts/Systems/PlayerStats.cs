using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerStats : MonoBehaviour
{
    public PersistantPlayerStats persistant { get; private set; }
    [SerializeField] public int level;
    public int mana { get; private set; }
    public int health { get; private set; }
    public float seconds { get; private set; } = 0f;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnManaChanged;
    private bool won = false;

    public static PlayerStats Instance { get; private set; }

    private void Update()
    {
        if (!won)
        {
            seconds += Time.deltaTime;
        }
    }

    private void Awake()
    {
        Instance = this;
        persistant = PersistantPlayerStats.Instance;
        mana = persistant.maxMana;
        health = persistant.maxHealth;
    }

    public void OnWin()
    {
        persistant.SetTime(seconds, level - 1);
        won = true;
    }

    /// <returns>Returns true if damage kills the player</returns>
    public bool DecreaseHealth(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            health = 0;
            RespawnManager.Instance.Die();
        }

        OnHealthChanged?.Invoke(health);
        return health <= 0;
    }

    public void IncreaseHealth(int amount)
    {
        if (health + amount > persistant.maxHealth)
        {
            health = persistant.maxHealth;
        }
        else
        {
            health += amount;
        }

        OnHealthChanged?.Invoke(health);
    }


    /// <returns>Returns true if mana was successfully reduced</returns>
    public bool DecreaseMana(int amount)
    {
        if (mana < amount)
        {
            return false;
        }

        mana -= amount;

        OnManaChanged?.Invoke(mana);
        return true;
    }

    public void IncreaseMana(int amount)
    {
        if (mana + amount > persistant.maxMana)
        {
            mana = persistant.maxMana;
        }
        else
        {
            mana += amount;
        }

        OnManaChanged?.Invoke(mana);
    }
}