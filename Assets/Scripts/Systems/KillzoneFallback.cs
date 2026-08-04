using UnityEngine;

public class KillzoneFallback : MonoBehaviour
{
    PlayerRef playerRef;
    private float killTimer;
    private bool triggered = false;

    private void Awake()
    {
        playerRef = GetComponent<PlayerRef>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerRef.playerMotor.linearVelocity.y <= -playerRef.playerStats.persistant.TERMINAL_VELOCITY + 0.1f)
        {
            killTimer += Time.deltaTime;
        }
        else
        {
            killTimer = 0f;
        }

        if (!triggered && killTimer >= 15f)
        {
            if (playerRef.checkGround.lastGroundedPosition.sqrMagnitude > 0.01 && PlayerStats.Instance.DecreaseMana(20))
            {
                playerRef.playerMotor.Teleport(playerRef.checkGround.lastGroundedPosition);
                playerRef.playerMotor.SetVelocity(Vector3.zero);
                triggered = true;
            }
            else
            {
                PlayerStats.Instance.DecreaseHealth(1000);
            }
        }
    }
}
