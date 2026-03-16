using UnityEngine;
using System.Collections;

public class MovementNEW : MonoBehaviour
{
    public enum AnimationType { Sit, SitLay, SitStand, SitPaw }

    public float moveSpeed = 1f;
    public float jumpForce = 3.5f;
    public float gravity = -9f;
    public float groundStickForce = -5f;
    public float coyoteTime = 0.15f;

    [Header("Running")]
    public float runSpeedMultiplier = 2f;      // Max speed multiplier at full run
    public float runAcceleration = 0.3f;       // How much each space tap increases intensity
    public float runDecay = 2f;                // Intensity lost per second
    public float maxRunIntensity = 1f;         // Maximum run intensity (added to walk)

    private CharacterController controller;
    public Animator animator;

    private Vector3 velocity;
    private float coyoteTimeCounter;
    private float jumpBlend;
    private bool isSitting;
    private bool isSitLay;
    private bool isSitStand;
    private bool isSitPaw;
    private bool isCrouchShock;
    private bool isLeftDodge;
    private bool isRightDodge;

    [HideInInspector]
    public bool isCrouchWalk;

    private bool jumpTrigger = false;
    private float jumpForceOverride = -1f;
    private float airSpeedMultiplier = 1f;

    private bool movementLocked = false;
    private bool autoMoveActive = false;
    private float autoMoveSpeed = 0f;
    private float autoMoveTimer = 0f;
    private float autoMoveDuration = 0.5f;

    private float moveX = 0f;
    private float runIntensity = 0f;

    private Coroutine currentAnimationCoroutine;
    private Coroutine dodgeCoroutine;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Normal player input (only when not locked and not auto‑moving)
        if (!movementLocked && !autoMoveActive)
        {
            moveX = 0f;
            if (Input.GetKey(KeyCode.D))
                moveX = 1f;

            // Running mechanic (only when moving right)
            if (moveX > 0f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    runIntensity = Mathf.Min(runIntensity + runAcceleration, maxRunIntensity);
                }
                // Decay run intensity over time
                runIntensity = Mathf.Max(runIntensity - runDecay * Time.deltaTime, 0f);
            }
            else
            {
                // Not moving right – decay faster
                runIntensity = Mathf.Max(runIntensity - runDecay * 2f * Time.deltaTime, 0f);
            }
        }
        else
        {
            // Locked or auto‑moving – decay run intensity
            runIntensity = Mathf.Max(runIntensity - runDecay * 2f * Time.deltaTime, 0f);
        }

        // Jump trigger from QTE
        if (jumpTrigger)
        {
            Jump();
            jumpTrigger = false;
        }

        // Animator updates
        if (animator != null)
        {
            // Set all boolean parameters
            animator.SetBool("Sit", isSitting);
            animator.SetBool("SitLay", isSitLay);
            animator.SetBool("SitStand", isSitStand);
            animator.SetBool("SitPaw", isSitPaw);
            animator.SetBool("CrouchWalk", isCrouchWalk);
            animator.SetBool("CrouchShock", isCrouchShock);
            animator.SetBool("LeftDodge", isLeftDodge);
            animator.SetBool("RightDodge", isRightDodge);

            // Determine the target walking float (0 = idle, 1 = walk, 2 = run)
            float targetWalk = 0f;

            if (!movementLocked && !autoMoveActive)
            {
                // Normal movement
                if (moveX > 0f)
                {
                    targetWalk = 1f + runIntensity; // ranges from 1 to 2
                }
                else
                {
                    targetWalk = 0f;
                }
            }
            else if (autoMoveActive)
            {
                // Automatic movement (QTE sequences)
                if (isCrouchWalk)
                {
                    targetWalk = 1f; // crouch walk uses walk blend
                }
                else if (isLeftDodge || isRightDodge)
                {
                    targetWalk = 0f; // dodge animations don't use walking float
                }
                else
                {
                    targetWalk = 1f; // fallback (e.g., jump lunge)
                }
            }

            // Smoothly blend the walking float
            float currentWalk = animator.GetFloat("isWalking");
            animator.SetFloat("isWalking", Mathf.MoveTowards(currentWalk, targetWalk, Time.deltaTime * 5f));

            // Jump blend
            bool isGrounded = controller.isGrounded;
            float targetJump = isGrounded ? 0f : 1f;
            jumpBlend = Mathf.MoveTowards(jumpBlend, targetJump, Time.deltaTime * 6f);
            animator.SetFloat("isJumping", jumpBlend);
        }

        // Auto‑move duration timer
        if (autoMoveActive)
        {
            autoMoveTimer -= Time.deltaTime;
            if (autoMoveTimer <= 0f)
            {
                autoMoveActive = false;
                if (movementLocked)
                    LockMovement(false);
            }
        }
    }

    void FixedUpdate()
    {
        bool isGrounded = controller.isGrounded;

        // Coyote time and ground stick
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

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;

        // Determine horizontal speed
        float horizontalSpeed = 0f;
        if (autoMoveActive)
        {
            horizontalSpeed = autoMoveSpeed;
        }
        else if (!movementLocked)
        {
            float speedMultiplier = 1f + runIntensity * (runSpeedMultiplier - 1f);
            horizontalSpeed = moveX * moveSpeed * speedMultiplier;
        }

        Vector3 move = new Vector3(horizontalSpeed, velocity.y, 0f);
        controller.Move(move * Time.fixedDeltaTime);

        // Keep Z position at 0
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
            runIntensity = 0f;
        }
    }

    public void PerformJumpLunge(float duration = 0.5f, float overrideJumpForce = -1f, float overrideAirSpeed = 1f)
    {
        if (overrideJumpForce > 0)
            jumpForceOverride = overrideJumpForce;

        airSpeedMultiplier = overrideAirSpeed;
        jumpTrigger = true;

        autoMoveActive = true;
        autoMoveSpeed = moveSpeed * overrideAirSpeed;
        autoMoveDuration = duration;
        autoMoveTimer = duration;
    }

    public void ActivateJump(float overrideJumpForce = -1f)
    {
        if (overrideJumpForce > 0)
            jumpForceOverride = overrideJumpForce;
        jumpTrigger = true;
    }

    // Crouch sequence
    public void StartCrouchSequence(float speed)
    {
        isCrouchWalk = true;
        isCrouchShock = false;

        movementLocked = true;
        autoMoveActive = true;
        autoMoveSpeed = speed;
        autoMoveDuration = 10000f;
        autoMoveTimer = autoMoveDuration;

        runIntensity = 0f;

        // Immediately set walking float to 1
        if (animator != null)
            animator.SetFloat("isWalking", 1f);
    }

    public void ResetCrouchWalk()
    {
        isCrouchWalk = false;
        if (animator != null)
            animator.SetFloat("isWalking", 0f);
    }

    // Left/right dodge sequence
    public void StartLeftRightSequence(float speed, bool startLeft, float interval, int maxSwaps)
    {
        // Reset crouch flags
        isCrouchWalk = false;
        isCrouchShock = false;

        // Set initial dodge
        isLeftDodge = startLeft;
        isRightDodge = !startLeft;

        movementLocked = true;
        autoMoveActive = true;
        autoMoveSpeed = speed;
        autoMoveDuration = 10000f;
        autoMoveTimer = autoMoveDuration;

        // Start limited‑swap coroutine
        if (dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
        dodgeCoroutine = StartCoroutine(AutoDodgeRoutine(interval, maxSwaps));
    }

    private IEnumerator AutoDodgeRoutine(float interval, int maxSwaps)
    {
        int swaps = 0;
        while (swaps < maxSwaps - 1) // We start with the first dodge, so we need (maxSwaps-1) swaps
        {
            yield return new WaitForSeconds(interval);
            // Swap left and right
            bool temp = isLeftDodge;
            isLeftDodge = isRightDodge;
            isRightDodge = temp;
            swaps++;
        }
        // After all swaps, just wait indefinitely (or until StopSequence is called)
        // This keeps the last dodge direction active.
        yield return new WaitForSeconds(float.MaxValue); // effectively pauses the coroutine
    }

    public void StopSequence()
    {
        if (dodgeCoroutine != null)
        {
            StopCoroutine(dodgeCoroutine);
            dodgeCoroutine = null;
        }

        isLeftDodge = false;
        isRightDodge = false;

        isSitting = false;
        isSitLay = false;
        isSitStand = false;
        isSitPaw = false;

        if (animator != null)
            animator.SetFloat("isWalking", 0f);

        runIntensity = 0f;

        autoMoveActive = false;
        movementLocked = false;
    }

    // Animation methods (unchanged)
    public void PlayAnimation(AnimationType type, float duration, float postLockDuration = 0f, bool standUpAtEnd = true)
    {
        if (currentAnimationCoroutine != null)
            StopCoroutine(currentAnimationCoroutine);

        switch (type)
        {
            case AnimationType.Sit:
                isSitLay = false; isSitStand = false; isSitPaw = false; isSitting = true;
                break;
            case AnimationType.SitLay:
                isSitStand = false; isSitPaw = false; isSitting = true; isSitLay = true;
                break;
            case AnimationType.SitStand:
                isSitLay = false; isSitPaw = false; isSitting = true; isSitStand = true;
                break;
            case AnimationType.SitPaw:
                isSitLay = false; isSitStand = false; isSitting = true; isSitPaw = true;
                break;
        }

        LockMovement(true);
        currentAnimationCoroutine = StartCoroutine(AnimationDurationRoutine(type, duration, postLockDuration, standUpAtEnd));
    }

    private IEnumerator AnimationDurationRoutine(AnimationType type, float duration, float postLockDuration, bool standUpAtEnd)
    {
        yield return new WaitForSeconds(duration);

        switch (type)
        {
            case AnimationType.Sit: break;
            case AnimationType.SitLay: isSitLay = false; break;
            case AnimationType.SitStand: isSitStand = false; break;
            case AnimationType.SitPaw: isSitPaw = false; break;
        }

        if (standUpAtEnd)
            isSitting = false;

        if (postLockDuration > 0)
            yield return new WaitForSeconds(postLockDuration);

        LockMovement(false);
        currentAnimationCoroutine = null;
    }
}