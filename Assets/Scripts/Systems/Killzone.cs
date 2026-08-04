using UnityEngine;

public class Killzone : MonoBehaviour
{
    private bool triggered = false;
    [SerializeField] private float triggerTime;
    private float triggerTimer = 0f;
    private Player player = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !triggered)
        {
            player = other.attachedRigidbody.gameObject.GetComponent<Player>();
            if (player && player.lastGrounded.sqrMagnitude > 0.01 && PlayerStats.Instance.DecreaseMana(20))
            {
                player.Motor.Teleport(player.lastGrounded);
                player.Motor.SetVelocity(Vector3.zero);
                triggered = true;
            }
            else
            {
                PlayerStats.Instance.DecreaseHealth(1000);
            }
        }
    }

    private void Update()
    {
        if (triggered && triggerTimer < triggerTime)
        {
            triggerTimer += Time.deltaTime;
        }

        if (triggerTimer > triggerTime)
        {
            triggered = false;
            triggerTimer = 0f;
        }
    }
}
