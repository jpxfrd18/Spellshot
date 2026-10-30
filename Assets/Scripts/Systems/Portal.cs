using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform portalTarget;
    private bool hitThisFrame = false;

    private void LateUpdate()
    {
        hitThisFrame = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitThisFrame && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = Player.Instance;

            if (player == null)
            {
                Debug.LogError("No player component exists");
                return;
            }

            player.Motor.Teleport(portalTarget.position);
            player.Motor.SetYaw(portalTarget.forward);
            player.Motor.SetVelocityDirection(portalTarget.forward);
            hitThisFrame = true;
        }
    }
}
