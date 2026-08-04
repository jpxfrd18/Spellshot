using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    //Grounding State
    public bool isGrounded { get; private set; } = false;
    public Vector3 groundNormal { get; private set; } = Vector3.zero;
    public float angle { get; private set; } = -1f;
    public bool triggerStayLastFrame { get; private set; } = false;
    public Vector3 lastGroundedPosition { get; private set; } = Vector3.zero;
    private HashSet<Collider> ground = null;

    //Stuck Handling
    [SerializeField] private float fallbackTime;
    private bool isStuck = false;
    private float fallbackTimer;
    private bool hovering = false;
    private bool forcedGrounding = false;
    private int teleportedCountdown = 0;

    //References
    private PlayerRef playerRef;
    private SphereCollider col;

    private void Awake()
    {
        playerRef = GetComponentInParent<PlayerRef>();
        col = GetComponent<SphereCollider>();
        fallbackTimer = fallbackTime;
        ground = new HashSet<Collider>();
    }

    private void Update()
    {
        triggerStayLastFrame = false;
    }

    private void FixedUpdate()
    {
        //Debug.Log("[Frame " + Time.frameCount + "] forcedGrounding = " + forcedGrounding);

        if (playerRef.playerMotor.linearVelocity.sqrMagnitude < 0.01 &&
          (playerRef.playerMovement.moveDirection.sqrMagnitude > 0.1 || playerRef.playerJump.jumpBufferCounter > 0))
        {
            if (playerRef.predictedWall.wallNormal.sqrMagnitude < 0.01)
            {
                isStuck = playerRef.predictedWall.triangleSoup;
                //Debug.Log("[Frame " + Time.frameCount + "] wall normal zero. isStuck = " + isStuck);
            }
            else if (Mathf.Abs(playerRef.predictedWall.wallNormal.y) < 0.1)
            {
                isStuck = Vector3.Dot(playerRef.playerMovement.moveDirection, playerRef.predictedWall.wallNormal) <= -0.99;
                //Debug.Log("[Frame " + Time.frameCount + "] wall normal horizontal. isStuck = " + isStuck);
                //Debug.Log("[Frame " + Time.frameCount + "] moveDirection = " + playerRef.playerMovement.moveDirection + ", wallNormal = " + playerRef.predictedWall.wallNormal);
            }
            else
            {
                //Debug.Log("[Frame " + Time.frameCount + "] wall normal not horizontal or 0. isStuck = true");
                isStuck = true;
            }
        }
        else if (isStuck)
        {
            isStuck = false;
            fallbackTimer = fallbackTime;
            hovering = true;
            playerRef.predictedWall.EnableFallbackAntiGravity();
        }

        if (isStuck)
        {
            fallbackTimer -= Time.fixedDeltaTime;

            if (fallbackTimer <= 0f)
            {
                TeleportFallback();
                teleportedCountdown = 10;
            }
        }
        else
        {
            fallbackTimer = fallbackTime;
        }

        if (hovering && teleportedCountdown <= 0)
        {
            if (playerRef.playerMotor.linearVelocity.sqrMagnitude > 1f || playerRef.playerMovement.moveDirection.sqrMagnitude > 0.1f || playerRef.playerJump.jumpBufferCounter > 0)
            {
                hovering = false;
                playerRef.predictedWall.DisableFallbackAntiGravity();
            }
        }

        if (teleportedCountdown > 0)
        {
            teleportedCountdown--;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (forcedGrounding)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.GetComponent<Ground>())
        {
            triggerStayLastFrame = true;
            Penetration(other);
        }


    }



    private void Penetration(Collider other)
    {
        if (Physics.ComputePenetration(col, col.transform.position, col.transform.rotation,
                                           other, other.transform.position, other.transform.rotation,
                                           out Vector3 direction, out float distance))
        {
            //Debug.Log("[Penetration] Frame: " + Time.frameCount + " Direction: " + direction + " Distance: " + distance);

            groundNormal = direction.normalized;

            angle = Vector3.Angle(groundNormal, Vector3.up);
            if (angle <= playerRef.playerStats.persistant.maxSlopeAngle && distance > 0.05f)
            {
                isGrounded = true;
                lastGroundedPosition = col.transform.TransformPoint(col.center) + Vector3.up * 1f;

                if (!ground.Contains(other))
                {
                    ground.Add(other);
                }
            }
            else
            {
                if (ground.Contains(other))
                {
                    ground.Remove(other);
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (forcedGrounding)
        {
            return;
        }

        if (ground.Contains(other))
        {
            ground.Remove(other);
        }

        if (ground.Count == 0)
        {
            isGrounded = false;
            angle = -1f;
            groundNormal = Vector3.zero;
        }
    }


    private void TeleportFallback()
    {
        Debug.Log("[Frame " + Time.frameCount + "] Teleport fallback");

        if (playerRef.predictedWall.wallNormal.sqrMagnitude > 0.01)
        {
            playerRef.playerMotor.Displace(0.1f * playerRef.predictedWall.wallNormal);
        }
        else
        {
            playerRef.playerMotor.Displace(0.1f * Vector3.up);
        }

        isGrounded = true;
        angle = 0f;
        groundNormal = Vector3.up;
        ground.Clear();
    }

    public void StartForcedGrounding()
    {
        if (forcedGrounding || !isGrounded)
        {
            return;
        }

        //Debug.Log("[Frame " + Time.frameCount + "] Forced grounding started");
        groundNormal = Vector3.zero;
        angle = 0f;
        forcedGrounding = true;
        ground.Clear();
    }

    public void EndForcedGrounding()
    {
        //Debug.Log("[Frame " + Time.frameCount + "] Forced grounding ended");

        forcedGrounding = false;
    }
}