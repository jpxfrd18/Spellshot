using UnityEngine;

public class RotatePitch : MonoBehaviour
{
    private PlayerRef playerRef;
    private float xRotation = 0;

    private void Awake()
    {
        playerRef = GetComponentInParent<PlayerRef>();
    }

    private void Update()
    {
        Vector2 look = playerRef.playerInput.lookAmt;

        float pitchDelta = look.y;

        xRotation -= pitchDelta;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
