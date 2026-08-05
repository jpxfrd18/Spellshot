using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform aimTransform;
    private PlayerRef playerRef;
    public static Player Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerRef = GetComponent<PlayerRef>();
    }

    public Vector3 AimForward => aimTransform.forward;
    public Vector3 AimSource => aimTransform.position;
    public PlayerStats Stats => playerRef.playerStats;
    public PlayerInputController Input => playerRef.playerInput;
    public PlayerMotor Motor => playerRef.playerMotor;
    public Vector3 lastGrounded => playerRef.checkGround.lastGroundedPosition;
}