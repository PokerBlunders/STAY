using UnityEngine;
using System.Collections;

public class MovementNEW : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 3.5f;
    public float gravity = -9f;
    public float groundStickForce = -5f;
    public float coyoteTime = 0.15f;

    private CharacterController controller;
    public Animator animator;

    private Vector3 velocity;
    private float coyoteTimeCounter;
    private float jumpBlend;
    private bool isSitting;
    private bool isLay;
    private bool isSitStand;
    private bool isSitPaw;

    private bool jumpTrigger = false;
    private float jumpForceOverride = -1f;
    private float airSpeedMultiplier = 1f;

    private bool movementLocked = false;
    private bool autoMoveActive = false;
    private float autoMoveTimer = 0f;
    private float autoMoveDuration = 0.5f;

    private float moveX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!movementLocked && !autoMoveActive)
        {
            moveX = 0f;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                moveX = 1f;
        }

        if (jumpTrigger)
        {
            Jump();
            jumpTrigger = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            isSitting = !isSitting;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            isLay = !isLay;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            isSitStand = !isSitStand;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            isSitPaw = !isSitPaw;

        if (animator != null)
        {
            animator.SetBool("Sit", isSitting);
            animator.SetBool("Lay", isLay);
            animator.SetBool("SitStand", isSitStand);
            animator.SetBool("SitPaw", isSitPaw);

            float targetWalk = (autoMoveActive || moveX > 0f) ? 1f : 0f;
            float currentWalk = animator.GetFloat("isWalking");
            animator.SetFloat("isWalking", Mathf.MoveTowards(currentWalk, targetWalk, Time.deltaTime * 5f));

            bool isGrounded = controller.isGrounded;
            float targetJump = isGrounded ? 0f : 1f;
            jumpBlend = Mathf.MoveTowards(jumpBlend, targetJump, Time.deltaTime * 6f);
            animator.SetFloat("isJumping", jumpBlend);
        }

        if (autoMoveActive)
        {
            autoMoveTimer -= Time.deltaTime;
            if (autoMoveTimer <= 0f)
            {
                autoMoveActive = false;
                airSpeedMultiplier = 1f;
                if (movementLocked)
                    LockMovement(false);
            }
        }
    }

    void FixedUpdate()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            if (velocity.y < 0)
                velocity.y = groundStickForce;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        velocity.y += gravity * Time.fixedDeltaTime;

        float horizontalSpeed = 0f;
        if (autoMoveActive)
            horizontalSpeed = moveSpeed * airSpeedMultiplier;
        else if (!movementLocked)
            horizontalSpeed = moveX * moveSpeed;

        Vector3 move = new Vector3(horizontalSpeed, velocity.y, 0f);
        controller.Move(move * Time.fixedDeltaTime);

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }

    void Jump()
    {
        bool isGrounded = controller.isGrounded;
        if (coyoteTimeCounter > 0f)
        {
            float force = (jumpForceOverride >= 0) ? jumpForceOverride : jumpForce;
            velocity.y = force;
            coyoteTimeCounter = 0f;
            isSitting = false;
            jumpForceOverride = -1f;
        }
    }

    public void LockMovement(bool locked)
    {
        movementLocked = locked;
        if (locked)
        {
            autoMoveActive = false;
            moveX = 0f;
            airSpeedMultiplier = 1f;
        }
    }

    public void PerformJumpLunge(float duration = 0.5f, float overrideJumpForce = -1f, float overrideAirSpeed = 1f)
    {
        if (overrideJumpForce > 0)
            jumpForceOverride = overrideJumpForce;

        airSpeedMultiplier = overrideAirSpeed;

        jumpTrigger = true;

        autoMoveActive = true;
        autoMoveDuration = duration;
        autoMoveTimer = duration;
    }

    public void ActivateJump(float overrideJumpForce = -1f)
    {
        if (overrideJumpForce > 0)
            jumpForceOverride = overrideJumpForce;
        jumpTrigger = true;
    }
}