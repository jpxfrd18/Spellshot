using UnityEngine;

public class EarlyWarning : MonoBehaviour
{
    private PlayerRef playerRef;
    public bool warning { get; private set; } = false;
    [SerializeField] private float warningVelocityThreshold;
    private SphereCollider col;

    private void Awake()
    {
        playerRef = GetComponentInParent<PlayerRef>();
        col = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.GetComponent<Ground>())
        {
            if (playerRef.checkGround.triggerStayLastFrame)
            {
                warning = false;
                return;
            }

            if (playerRef.playerMotor.linearVelocity.y >= warningVelocityThreshold)
            {
                warning = false;
                return;
            }

            Vector3 closestPoint = other.ClosestPoint(transform.position);

            if (closestPoint.y + 1f > col.transform.TransformPoint(col.center).y)
            {
                warning = false;
                return;
            }


            warning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            warning = false;
        }
    }
}
