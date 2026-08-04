using UnityEngine;

public class PlayerRef : MonoBehaviour
{
    public PlayerStats playerStats { get; private set; }
    public PauseController pauseController { get; private set; }

    [field: SerializeField] public EarlyWarning earlyWarning { get; private set; }
    [field: SerializeField] public CheckGround checkGround { get; private set; }
    [field: SerializeField] public PlayerInputController playerInput { get; private set; }
    [field: SerializeField] public PlayerJumpController playerJump { get; private set; }
    [field: SerializeField] public PlayerMotor playerMotor { get; private set; }
    [field: SerializeField] public PlayerMovementController playerMovement { get; private set; }
    [field: SerializeField] public SpellController spellController { get; private set; }
    [field: SerializeField] public PredictedWall predictedWall { get; private set; }
    [field: SerializeField] public RotateYaw rotateYaw { get; private set; }

    public void Initialize(PlayerStats stats, PauseController pause)
    {
        playerStats = stats;
        pauseController = pause;
    }
}