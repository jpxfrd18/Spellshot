using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    public bool canJump => !playerRef.playerMotor.justJumped &&
    (playerRef.earlyWarning.warning || playerRef.checkGround.isGrounded || coyoteCounter > 0f);

    public float jumpBufferCounter { get; private set; } = 0f;
    private PlayerRef playerRef;
    private float coyoteCounter = 0f;
    private bool wasGroundedLastFrame = false;

    private void Awake()
    {
        playerRef = GetComponent<PlayerRef>();
    }

    // Update is called once per frame
    void Update()
    {
        wasGroundedLastFrame = playerRef.checkGround.isGrounded;

        Timers();
    }

    void FixedUpdate()
    {
        checkGround();
    }

    private void Timers()
    {
        if (playerRef.playerInput.jumpAction.triggered)
        {
            jumpBufferCounter = playerRef.playerStats.persistant.JUMP_BUFFER;
            //Debug.Log("[Frame " + Time.frameCount + "] isGrounded = " + playerRef.checkGround.isGrounded);
        }

        if (playerRef.playerInput.jumpAction.IsPressed() && canJump)
        {
            jumpBufferCounter = playerRef.playerStats.persistant.JUMP_BUFFER;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (!playerRef.checkGround.isGrounded && coyoteCounter > 0)
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private void checkGround()
    {
        if (wasGroundedLastFrame && !playerRef.checkGround.isGrounded && !playerRef.playerMotor.justJumped)
        {
            coyoteCounter = playerRef.playerStats.persistant.COYOTE_TIME;
            Debug.Log("[Frame " + Time.frameCount + "] Coyote time activated");
        }
    }

    public void Reset()
    {
        jumpBufferCounter = 0;
        coyoteCounter = 0;
    }
}
