using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class PersistantPlayerStats : MonoBehaviour
{
    [field: SerializeField] public float sensitivity { get; private set; }
    public float maxSpeedGround { get; private set; } = 6f;
    public float maxSpeedAir { get; private set; } = 20f;
    public float launchSpeed { get; private set; } = 30f;
    public float groundAcceleration { get; private set; } = 300f;
    public float airAcceleration { get; private set; } = 1f;
    public float maxSlopeAngle { get; private set; } = 45f;
    public float turnStrength { get; private set; } = 5f;
    public float jumpStrength { get; private set; } = 9f;
    public float catchUpMultiplier { get; private set; } = 3f;
    public float JUMP_BUFFER { get; private set; } = 0.1f;
    public float COYOTE_TIME { get; private set; } = 0.15f;
    public float FRICTION_BUFFER { get; private set; } = 0.1f;
    public float TERMINAL_VELOCITY { get; private set; } = 35f;
    public float MAX_VERTICAL_VELOCITY { get; private set; } = 30f;
    public float GRAVITY_DOWN { get; private set; } = 15f;
    public float GRAVITY_UP { get; private set; } = 10f;
    public int maxMana { get; private set; } = 100;
    public int maxHealth { get; private set; } = 100;
    public float musicVolume { get; private set; } = 0.5f;
    public event Action<float> OnMusicVolumeChanged;
    public List<float> bestTime { get; private set; } = new List<float>();
    [field: SerializeField] public int latestLevelUnlocked { get; private set; } = 1;
    [SerializeField] private List<SpellType> unlockedSpellsInspector = new List<SpellType>();
    public HashSet<SpellType> unlockedSpells { get; private set; }
    [field: SerializeField] public SpellType[] spells { get; private set; } = new SpellType[4];
    public string[] bindingNames { get; private set; }
    public string[] controllerBindingNames { get; private set; }

    public static PersistantPlayerStats Instance { get; private set; }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        unlockedSpells = new HashSet<SpellType>(unlockedSpellsInspector);

        bindingNames = new string[]
        {
            "Mouse Left",
            "Mouse Right",
            "Q",
            "E",
            "Space",
            "Escape",
            "R"
        };

        controllerBindingNames = new string[]
        {
            "LT",
            "RT",
            "LB",
            "RB",
            "A",
            "Start",
            "D-Pad (any)"
        };
    }

    public void LevelComplete(int level)
    {
        //Temporary
        if (level == 2)
        {
            return;
        }

        if (level == latestLevelUnlocked)
        {
            latestLevelUnlocked++;
        }
    }

    public void UpdateControllerBindingName(int index, string newName)
    {
        if (index < 0 || index >= controllerBindingNames.Length)
        {
            Debug.LogError("Index " + index + " is out of bounds for controllerBindingNames array.");
            return;
        }

        controllerBindingNames[index] = newName;
    }

    public void UpdateBindingName(int index, string newName)
    {
        if (index < 0 || index >= bindingNames.Length)
        {
            Debug.LogError("Index " + index + " is out of bounds for bindingNames array.");
            return;
        }

        bindingNames[index] = newName;
    }

    public void UnlockSpell(SpellType spellType)
    {
        if (unlockedSpells.Contains(spellType))
        {
            return;
        }

        unlockedSpells.Add(spellType);
    }

    public int ContainsSpell(SpellType spellType)
    {
        for (int i = 0; i < 4; i++)
        {
            if (spells[i] == spellType)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes a spell from the selected loadout
    /// </summary>
    /// <param name="spellType">The type of spell to remove</param>
    public void RemoveSpell(SpellType spellType)
    {
        for (int i = 0; i < spells.Length; i++)
        {
            if (spells[i] == spellType)
            {
                spells[i] = SpellType.None;
                return;
            }
        }
    }

    public void SetTime(float time, int index)
    {
        if (index < 0)
        {
            return;
        }

        if (index == bestTime.Count)
        {
            bestTime.Add(time);
            return;
        }

        if (index > bestTime.Count)
        {
            while (bestTime.Count < index)
            {
                bestTime.Add(-1f);
            }
            bestTime.Add(time);
            return;
        }

        if (bestTime[index] < 0 || time < bestTime[index])
        {
            bestTime[index] = time;
        }
    }

    public void IncreaseMusicVolume(float amount)
    {
        float newVolume = Mathf.Clamp01(musicVolume + amount);

        if (Mathf.Approximately(newVolume, musicVolume))
        {
            return; // No change in volume, so don't invoke the event
        }

        musicVolume = newVolume;
        OnMusicVolumeChanged?.Invoke(musicVolume);
    }

    public void DecreaseMusicVolume(float amount)
    {
        float newVolume = Mathf.Clamp01(musicVolume - amount);

        if (Mathf.Approximately(newVolume, musicVolume))
        {
            return; // No change in volume, so don't invoke the event
        }

        musicVolume = newVolume;
        OnMusicVolumeChanged?.Invoke(musicVolume);
    }

    public void SetMusicVolume(float newVolume)
    {
        float clampedVolume = Mathf.Clamp01(newVolume);

        if (Mathf.Approximately(clampedVolume, musicVolume))
        {
            return; // No change in volume, so don't invoke the event
        }

        musicVolume = clampedVolume;
        OnMusicVolumeChanged?.Invoke(musicVolume);
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (maxHealth + amount > 2000)
        {
            //Failsafe
            maxHealth = 2000;
            return;
        }
        maxHealth += amount;
    }

    public void DecreaseMaxHealth(int amount)
    {
        if (maxHealth - amount <= 1)
        {
            //Failsafe
            maxHealth = 1;
            return;
        }

        maxHealth -= amount;
    }

    public void IncreaseMaxMana(int amount)
    {
        if (maxMana + amount > 2000)
        {
            //Failsafe
            maxMana = 2000;
            return;
        }

        maxMana += amount;
    }

    /// <returns>Returns true if max mana was successfully reduced</returns>
    public bool DecreaseMaxMana(int amount)
    {
        if (maxMana - amount <= 0)
        {
            return false;
        }

        maxMana -= amount;
        return true;
    }


    public void IncreaseSensitivity(float amount)
    {
        if (sensitivity + amount > 0.4f)
        {
            sensitivity = 0.4f;
            return;
        }

        sensitivity += amount;
    }

    public void DecreaseSensitivity(float amount)
    {
        if (sensitivity - amount < 0.05f)
        {
            sensitivity = 0.05f;
            return;
        }

        sensitivity -= amount;
    }
}