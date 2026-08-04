using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    // --- Serialized Materials ---
    [SerializeField] private PhysicsMaterial slippery;
    [SerializeField] private PhysicsMaterial sticky;

    // --- Public Properties ---
    public bool justJumped { get; private set; } = false;
    public Vector3 linearVelocity => rb.linearVelocity;

    // --- Component References ---
    private Rigidbody rb;
    private CapsuleCollider col;
    private PlayerRef playerRef;

    // --- State Flags ---
    private bool wasGroundedLastFrame = false;
    private bool forceUnground = false;

    // --- Timers ---
    private float frictionTimer = 0f;
    private float launchTimer = 0f;
    private float forceUngroundTimer = 0f;

    // --- Cached Values ---
    private Vector3 wallNormalLastFrame = Vector3.zero;
    private Vector3 jumpImpulse = Vector3.zero;


    private void Awake()
    {
        playerRef = GetComponent<PlayerRef>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        rb.isKinematic = playerRef.pauseController.isPaused;

        if (frictionTimer > 0)
        {
            frictionTimer -= Time.deltaTime;
        }

        if (launchTimer > 0)
        {
            launchTimer -= Time.deltaTime;
        }

        if (forceUngroundTimer > 0)
        {
            forceUngroundTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // --- Friction Material Management ---
        if (playerRef.playerMovement.moveDirection.sqrMagnitude > 0.01)
        {
            col.material = slippery;
        }
        else
        {
            col.material = sticky;
        }

        // --- Friction Timer Setup ---
        if (!wasGroundedLastFrame && playerRef.checkGround.isGrounded)
        {
            frictionTimer = playerRef.playerStats.persistant.FRICTION_BUFFER;
        }

        // --- Core Physics ---
        Gravity();
        Jump();

        // --- Unground State ---
        if (forceUngroundTimer <= 0 || (justJumped && !playerRef.checkGround.isGrounded))
        {
            forceUnground = false;
            justJumped = false;
        }

        // --- Linear Damping (Friction Control) ---
        if (!forceUnground && playerRef.checkGround.isGrounded && frictionTimer <= 0f && launchTimer <= 0f)
        {
            rb.linearDamping = 10f;
        }
        else
        {
            rb.linearDamping = 0.05f;
        }

        // --- Leave Ground Adjustments ---
        if (wasGroundedLastFrame && !playerRef.checkGround.isGrounded)
        {
            rb.linearDamping = 0.05f; // Ensure friction is removed immediately upon leaving the ground

            if (rb.linearVelocity.y > 0 && !justJumped && launchTimer <= 0f && playerRef.checkGround.isGrounded)
            {
                Debug.Log("[Frame " + Time.frameCount + "] Slowdown");
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.3f, rb.linearVelocity.z); // Slow upward momentum from slope boosting if you don't jump
            }
        }

        // --- Terminal Velocity Cap ---
        if (rb.linearVelocity.y < -playerRef.playerStats.persistant.TERMINAL_VELOCITY)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -playerRef.playerStats.persistant.TERMINAL_VELOCITY, rb.linearVelocity.z);
        }

        if (rb.linearVelocity.y > playerRef.playerStats.persistant.MAX_VERTICAL_VELOCITY && launchTimer <= 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, playerRef.playerStats.persistant.MAX_VERTICAL_VELOCITY, rb.linearVelocity.z);
        }

        // --- Movement Execution ---
        Move();

        // --- State Updates ---
        wasGroundedLastFrame = playerRef.checkGround.isGrounded;
        justJumped = justJumped ? playerRef.checkGround.isGrounded : false;

        // --- Wall Detection ---
        if (playerRef.playerMovement.moveDirection.sqrMagnitude > 0.01f && playerRef.predictedWall.wallNormal.sqrMagnitude > 0.01f)
        {
            wallNormalLastFrame = playerRef.predictedWall.wallNormal;
            wallNormalLastFrame.y = 0;
            wallNormalLastFrame.Normalize();
        }
        else if (playerRef.playerMovement.moveDirection.sqrMagnitude > 0.01f &&
        Vector3.Dot(playerRef.playerMovement.moveDirection, wallNormalLastFrame) > 0)
        {
            wallNormalLastFrame = Vector3.zero;
        }

        // --- Debug --- 
        /*
        Debug.Log("[Frame " + Time.frameCount + "] Grounded: " + playerRef.checkGround.isGrounded
        + " Force Unground Timer: " + forceUngroundTimer
        + " Force Unground: " + forceUnground
        + " Just Jumped: " + justJumped
        + " Linear Damping: " + rb.linearDamping
        + " Y Position: " + rb.position.y);
        */

        //Debug.Log("[Frame " + Time.frameCount + "] Wall Normal: " + playerRef.predictedWall.wallNormal);
    }

    private void Move()
    {
        if (launchTimer > 0)
        {
            AirMove(playerRef.playerStats.persistant.launchSpeed);
        }
        else if (!forceUnground && playerRef.checkGround.isGrounded && frictionTimer <= 0f)
        {
            GroundMove();
        }
        else
        {
            AirMove(playerRef.playerStats.persistant.maxSpeedAir);
        }
    }

    private void GroundMove()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 groundMoveDirection = new Vector3(playerRef.playerMovement.moveDirection.x, 0, playerRef.playerMovement.moveDirection.z);
        float maxSpeed = playerRef.playerStats.persistant.maxSpeedGround;

        if (playerRef.checkGround.angle > 0.1f && playerRef.checkGround.angle <= playerRef.playerStats.persistant.maxSlopeAngle)
        {
            groundMoveDirection = Quaternion.AngleAxis(playerRef.checkGround.angle * 0.2f, Vector3.Cross(Vector3.up, playerRef.checkGround.groundNormal)) * playerRef.playerMovement.moveDirection;
        }

        Vector3 groundForce = groundMoveDirection.normalized * playerRef.playerStats.persistant.groundAcceleration;

        Vector3 horizontalWallNormal = playerRef.predictedWall.wallNormal;
        horizontalWallNormal.y = 0;
        horizontalWallNormal.Normalize();

        if (horizontalWallNormal.sqrMagnitude <= 0.01f)
        {
            horizontalWallNormal = wallNormalLastFrame;
        }

        // Remove the component of movement in the downslope direction
        float dot = Vector3.Dot(horizontalWallNormal, groundForce.normalized);

        if (dot > 0f)
        {
            Vector3 badVelocity = dot * horizontalWallNormal;
            Vector3 goodVelocity = groundForce - badVelocity;

            groundForce = goodVelocity;
        }


        rb.AddForce(groundForce, ForceMode.Acceleration);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }

        playerRef.predictedWall.PredictPosition(rb.linearVelocity, jumpImpulse, groundForce, rb.linearDamping);
    }

    private void AirMove(float maxSpeed)
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 horizontalMoveDirection = new Vector3(playerRef.playerMovement.moveDirection.x, 0, playerRef.playerMovement.moveDirection.z);

        float turnAngle;
        float extraTurn = 0f;

        if (horizontalVelocity.magnitude > 0)
        {
            float dotProduct = Vector3.Dot(horizontalVelocity.normalized, horizontalMoveDirection.normalized);
            dotProduct = Mathf.Clamp(dotProduct, -1f, 1f); // Clamp to [-1, 1] to avoid NaN from Acos
            turnAngle = Mathf.Acos(dotProduct);
            if (turnAngle > 0.2f) // Only apply turn strength if there's a significant direction change
                extraTurn = turnAngle * playerRef.playerStats.persistant.turnStrength;
        }

        Vector3 airForce = playerRef.playerMovement.moveDirection * (playerRef.playerStats.persistant.airAcceleration + extraTurn);

        Vector3 horizontalWallNormal = playerRef.predictedWall.wallNormal;
        horizontalWallNormal.y = 0;
        horizontalWallNormal.Normalize();

        if (horizontalWallNormal.sqrMagnitude <= 0.01f)
        {
            horizontalWallNormal = wallNormalLastFrame;
        }

        // Remove the component of movement in the downslope direction
        float dot = Vector3.Dot(horizontalWallNormal, airForce.normalized);

        if (dot > 0f)
        {
            Vector3 badVelocity = dot * horizontalWallNormal;
            Vector3 goodVelocity = airForce - badVelocity;

            airForce = goodVelocity;
        }


        if (horizontalVelocity.magnitude < playerRef.playerStats.persistant.maxSpeedGround)
        {
            airForce *= playerRef.playerStats.persistant.catchUpMultiplier;
        }

        rb.AddForce(airForce, ForceMode.Acceleration);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }

        playerRef.predictedWall.PredictPosition(rb.linearVelocity, jumpImpulse, airForce, rb.linearDamping);
    }

    private void Jump()
    {
        if (playerRef.playerJump.jumpBufferCounter > 0 && playerRef.playerJump.canJump)
        {
            //Debug.Log("[Frame " + Time.frameCount + "] Jumping");
            //Debug.Log("[Frame " + Time.frameCount + "] Grounded: " + playerRef.checkGround.isGrounded + " Early Warning: " + playerRef.earlyWarning.warning);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset vertical velocity before jumping to ensure consistent jump height
            rb.AddForce(Vector3.up * playerRef.playerStats.persistant.jumpStrength, ForceMode.Impulse);
            rb.linearDamping = 0.05f; // Reduce friction immediately upon jumping
            playerRef.playerJump.Reset();
            justJumped = true;
            forceUnground = true;
            forceUngroundTimer = 0.5f;
            jumpImpulse = Vector3.up * playerRef.playerStats.persistant.jumpStrength; // Store the jump impulse for prediction

        }
        else
        {
            jumpImpulse = Vector3.zero; // Clear the jump impulse if not jumping this frame
        }
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            Physics.gravity = Vector3.down * playerRef.playerStats.persistant.GRAVITY_DOWN;
        }
        else
        {
            Physics.gravity = Vector3.down * playerRef.playerStats.persistant.GRAVITY_UP;
        }
    }

    public void Launch(Vector3 direction)
    {
        Vector3 launchDirection = direction.normalized;
        launchDirection.Normalize();

        if (playerRef.playerJump.canJump && launchDirection.y > 0)
        {
            playerRef.playerJump.Reset();
            rb.linearDamping = 0.05f;
            justJumped = true;
            forceUnground = true;
            forceUngroundTimer = 0.5f;
        }
        else if (playerRef.checkGround.isGrounded && launchDirection.y < 0.1f)
        {
            launchDirection.y = 0.1f;
            launchDirection.Normalize();
        }

        SetVelocity(launchDirection * playerRef.playerStats.persistant.launchSpeed);
        launchTimer = 1f;
    }

    public void SetVelocity(Vector3 velocity)
    {
        rb.linearVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
    }

    public void Teleport(Vector3 position)
    {
        rb.position = position;
        playerRef.playerJump.Reset();
        rb.linearDamping = 0.05f;
    }

    public void SetYaw(Vector3 forward)
    {
        playerRef.rotateYaw.SetRotation(forward);
    }

    public void SetVelocityDirection(Vector3 forward)
    {
        float vy = rb.linearVelocity.y;
        float horizontalMagnitude = Mathf.Sqrt(Mathf.Pow(rb.linearVelocity.x, 2) + Mathf.Pow(rb.linearVelocity.z, 2));

        Vector3 newVelocity = forward * horizontalMagnitude;
        newVelocity.y = vy;

        rb.linearVelocity = newVelocity;
    }

    public void MultiplyVelocity(float amount)
    {
        float vx = rb.linearVelocity.x * amount;
        float vz = rb.linearVelocity.z * amount;

        rb.linearVelocity = new Vector3(vx, rb.linearVelocity.y, vz);
    }

    public void Displace(Vector3 displacement)
    {
        rb.position += displacement;
        playerRef.playerJump.Reset();
        rb.linearDamping = 0.05f;
    }

    public void Gust()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, 0f), rb.linearVelocity.z);
        rb.AddForce(Vector3.up * playerRef.playerStats.persistant.jumpStrength * 1.5f, ForceMode.Impulse);

        if (playerRef.playerJump.canJump)
        {
            playerRef.playerJump.Reset();
            rb.linearDamping = 0.05f;
            justJumped = true;
            forceUnground = true;
            forceUngroundTimer = 0.5f;
        }
    }
}
