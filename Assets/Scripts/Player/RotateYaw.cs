using UnityEngine;

public class RotateYaw : MonoBehaviour
{
    private PlayerRef playerRef;
    private float yRotation;
    private bool initialized;

    public void Init(float initialRotation)
    {
        if (!initialized)
        {
            yRotation = initialRotation;
        }
    }

    private void Awake()
    {
        playerRef = GetComponentInParent<PlayerRef>();
    }

    private void Update()
    {
        Vector2 look = playerRef.playerInput.lookAmt;

        float yawDelta = look.x * playerRef.playerStats.persistant.sensitivity;

        yRotation += yawDelta;

    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void SetRotation(Vector3 forward)
    {
        float yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        yRotation = yaw;
    }
}
