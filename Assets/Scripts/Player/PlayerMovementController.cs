using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Transform body;
    public Vector3 moveDirection { get; private set; }
    private PlayerRef playerRef;

    private void Awake()
    {
        playerRef = GetComponent<PlayerRef>();
    }

    private void Update()
    {
        CalculateMoveDirection();
    }

    private void CalculateMoveDirection()
    {
        moveDirection = body.forward * playerRef.playerInput.moveAmt.y + body.right * playerRef.playerInput.moveAmt.x;
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z); // Ensure movement is horizontal
        moveDirection.Normalize();
    }
}
