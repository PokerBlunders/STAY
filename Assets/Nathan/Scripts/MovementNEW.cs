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
    public float runSpeedMultiplier = 2f;
    public float runAcceleration = 0.3f;
    public float runDecay = 2f;
    public float maxRunIntensity = 1f;

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
        if (!movementLocked && !autoMoveActive)
        {
            moveX = 0f;
            if (Input.GetKey(KeyCode.D))
                moveX = 1f;

            if (moveX > 0f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    runIntensity = Mathf.Min(runIntensity + runAcceleration, maxRunIntensity);
                }
                runIntensity = Mathf.Max(runIntensity - runDecay * Time.deltaTime, 0f);
            }
            else
            {
                runIntensity = Mathf.Max(runIntensity - runDecay * 2f * Time.deltaTime, 0f);
            }
        }
        else
        {
            runIntensity = Mathf.Max(runIntensity - runDecay * 2f * Time.deltaTime, 0f);
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
            animator.SetBool("CrouchWalk", isCrouchWalk);
            animator.SetBool("CrouchShock", isCrouchShock);
            animator.SetBool("LeftDodge", isLeftDodge);
            animator.SetBool("RightDodge", isRightDodge);

            float targetWalk = 0f;

            if (!movementLocked && !autoMoveActive)
            {
                if (moveX > 0f)
                {
                    targetWalk = 1f + runIntensity;
                }
                else
                {
                    targetWalk = 0f;
                }
            }
            else if (autoMoveActive)
            {
                if (isCrouchWalk)
                {
                    targetWalk = 1f;
                }
                else if (isLeftDodge || isRightDodge)
                {
                    targetWalk = 0f;
                }
                else
                {
                    targetWalk = 1f;
                }
            }

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

        if (animator != null)
            animator.SetFloat("isWalking", 1f);
    }

    public void ResetCrouchWalk()
    {
        isCrouchWalk = false;
        if (animator != null)
            animator.SetFloat("isWalking", 0f);
    }

    public void StartLeftRightSequence(float speed, bool startLeft, float interval, int maxSwaps)
    {
        isCrouchWalk = false;
        isCrouchShock = false;

        isLeftDodge = startLeft;
        isRightDodge = !startLeft;

        movementLocked = true;
        autoMoveActive = true;
        autoMoveSpeed = speed;
        autoMoveDuration = 10000f;
        autoMoveTimer = autoMoveDuration;

        if (dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
        dodgeCoroutine = StartCoroutine(AutoDodgeRoutine(interval, maxSwaps));
    }

    private IEnumerator AutoDodgeRoutine(float interval, int maxSwaps)
    {
        int swaps = 0;
        while (swaps < maxSwaps - 1)
        {
            yield return new WaitForSeconds(interval);
            bool temp = isLeftDodge;
            isLeftDodge = isRightDodge;
            isRightDodge = temp;
            swaps++;
        }

        yield return new WaitForSeconds(float.MaxValue);
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