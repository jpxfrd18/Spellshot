using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private float controllerMultiplier;
    private PlayerRef playerRef;

    public InputAction jumpAction { get; private set; }
    public Vector2 lookAmt { get; private set; }
    public Vector2 moveAmt { get; private set; }

    public event Action<int> OnSpellPressed;
    public event Action<int> OnSpellReleased;

    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction spell1Action;
    private InputAction spell2Action;
    private InputAction spell3Action;
    private InputAction spell4Action;
    private bool spell1Held;
    private bool spell2Held;
    private bool spell3Held;
    private bool spell4Held;
    private InputDevice device;

    private void Awake()
    {
        lookAction = InputActions.FindAction("Look");
        moveAction = InputActions.FindAction("Move");
        jumpAction = InputActions.FindAction("Jump");
        spell1Action = InputActions.FindAction("Spell1");
        spell2Action = InputActions.FindAction("Spell2");
        spell3Action = InputActions.FindAction("Spell3");
        spell4Action = InputActions.FindAction("Spell4");

        spell1Action.started += ctx => OnSpellPressed?.Invoke(0);
        spell1Action.canceled += ctx => OnSpellReleased?.Invoke(0);

        spell2Action.started += ctx => OnSpellPressed?.Invoke(1);
        spell2Action.canceled += ctx => OnSpellReleased?.Invoke(1);

        spell3Action.started += ctx => OnSpellPressed?.Invoke(2);
        spell3Action.canceled += ctx => OnSpellReleased?.Invoke(2);

        spell4Action.started += ctx => OnSpellPressed?.Invoke(3);
        spell4Action.canceled += ctx => OnSpellReleased?.Invoke(3);

        playerRef = GetComponent<PlayerRef>();
    }


    private void Update()
    {
        spell1Held = spell1Action.IsPressed();
        spell2Held = spell2Action.IsPressed();
        spell3Held = spell3Action.IsPressed();
        spell4Held = spell4Action.IsPressed();

        moveAmt = moveAction.ReadValue<Vector2>();

        device = lookAction.activeControl?.device;
        if (device == null)
        {
            lookAmt = Vector2.zero;
            return;
        }

        if (device is Mouse)
        {
            lookAmt = lookAction.ReadValue<Vector2>() * playerRef.playerStats.persistant.sensitivity;
        }
        else
        {
            lookAmt = lookAction.ReadValue<Vector2>() * playerRef.playerStats.persistant.sensitivity * controllerMultiplier;
        }
    }

    public bool getSpellHeld(int index)
    {
        if (index == 0)
        {
            return spell1Held;
        }
        else if (index == 1)
        {
            return spell2Held;
        }
        else if (index == 2)
        {
            return spell3Held;
        }
        else if (index == 3)
        {
            return spell4Held;
        }

        return false;
    }
}