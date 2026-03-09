using UnityEngine;
using System.Collections;

public class MovementNEW : MonoBehaviour
{
    // Animation types that can be triggered externally
    public enum AnimationType { Sit, SitLay, SitStand, SitPaw }

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
    private bool isSitLay;
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

    // Animation coroutine reference
    private Coroutine currentAnimationCoroutine;

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


        if (animator != null)
        {
            animator.SetBool("Sit", isSitting);
            animator.SetBool("SitLay", isSitLay);
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

    // Play an animation, lock movement for its duration, and optionally stand up at the end
    public void PlayAnimation(AnimationType type, float duration, float postLockDuration = 0f, bool standUpAtEnd = true)
    {
        // Stop any currently playing animation coroutine
        if (currentAnimationCoroutine != null)
            StopCoroutine(currentAnimationCoroutine);

        // Reset all special flags, set the chosen one correctly
        switch (type)
        {
            case AnimationType.Sit:
                isSitLay = false;
                isSitStand = false;
                isSitPaw = false;
                isSitting = true;
                break;

            case AnimationType.SitLay:
                isSitStand = false;
                isSitPaw = false;
                isSitting = true;   // sit required before lay
                isSitLay = true;
                break;

            case AnimationType.SitStand:
                isSitLay = false;
                isSitPaw = false;
                isSitting = true;
                isSitStand = true;
                break;

            case AnimationType.SitPaw:
                isSitLay = false;
                isSitStand = false;
                isSitting = true;
                isSitPaw = true;
                break;
        }

        LockMovement(true);
        currentAnimationCoroutine = StartCoroutine(AnimationDurationRoutine(type, duration, postLockDuration, standUpAtEnd));
    }

    private IEnumerator AnimationDurationRoutine(AnimationType type, float duration, float postLockDuration, bool standUpAtEnd)
    {
        yield return new WaitForSeconds(duration);

        // After duration, reset the triggered animation appropriately
        switch (type)
        {
            case AnimationType.Sit:
                // Nothing to reset – we'll handle stand later
                break;
            case AnimationType.SitLay:
                isSitLay = false;
                // isSitting remains true
                break;
            case AnimationType.SitStand:
                isSitStand = false;
                // isSitting remains true
                break;
            case AnimationType.SitPaw:
                isSitPaw = false;
                // isSitting remains true
                break;
        }

        if (standUpAtEnd)
        {
            isSitting = false;
        }

        // Extra lock time after the animation
        if (postLockDuration > 0)
            yield return new WaitForSeconds(postLockDuration);

        LockMovement(false);
        currentAnimationCoroutine = null;
    }
}