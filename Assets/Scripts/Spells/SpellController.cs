using UnityEngine;
using UnityEngine.InputSystem;

public class SpellController : MonoBehaviour
{
    [SerializeField] private SpellModule[] spellModules;
    private SpellModule[] equippedModules = new SpellModule[4];
    private Player player;

    private bool[] triggerGate = new bool[4];

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        player.Input.OnSpellPressed += HandleSpellPressed;
        player.Input.OnSpellReleased += HandleSpellReleased;

        if (spellModules[0] != null)
        {
            Debug.LogError("Index 0 of spellModules must be null (SpellType.None)");
        }

        for (int i = 0; i < 4; i++)
        {
            SpellType type = player.Stats.persistant.spells[i];

            if (type == SpellType.None || (int)type >= spellModules.Length)
            {
                equippedModules[i] = null;
                continue;
            }

            SpellModule prefab = spellModules[(int)type];

            if (prefab == null)
            {
                Debug.LogWarning("No prefab assigned for spell type " + type + ".");
                equippedModules[i] = null;
            }
            else
            {
                equippedModules[i] = Instantiate(prefab, transform);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            SpellModule module = equippedModules[i];
            if (module == null)
            {
                continue;
            }

            if (module.CastType == SpellCastType.Hold && player.Input.getSpellHeld(i))
            {
                module.Cast(player);
            }
        }
    }

    private void HandleSpellPressed(int index)
    {
        if (triggerGate[index] &&
        (player.Stats.persistant.controllerBindingNames[index].Equals("RT") || player.Stats.persistant.controllerBindingNames[index].Equals("LT"))
        && Gamepad.current != null)
        {
            triggerGate[index] = false;
            return;
        }

        triggerGate[index] = true;

        SpellModule module = equippedModules[index];
        if (module == null)
        {
            return;
        }

        if (module.CastType == SpellCastType.Press)
        {
            module.Cast(player);
        }
    }

    private void HandleSpellReleased(int index)
    {

    }
}
