using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -25f;
    public float groundStickForce = -5f;
    public float coyoteTime = 0.15f;
    public float jumpCooldown = 0.2f;

    private CharacterController controller;
    public Animator animator;

    private Vector3 velocity;
    private float coyoteTimeCounter;
    private float jumpBlend;
    private bool isSitting;
    private float jumpCooldownTimer;

    private float moveX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveX = 0f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            isSitting = !isSitting;
        }

        if (animator != null)
        {
            animator.SetBool("Sit", isSitting);

            float targetWalk = moveX;
            float currentWalk = animator.GetFloat("isWalking");
            animator.SetFloat("isWalking", Mathf.MoveTowards(currentWalk, targetWalk, Time.deltaTime * 5f));

            bool isGrounded = controller.isGrounded;
            float targetJump = isGrounded ? 0f : 1f;
            jumpBlend = Mathf.MoveTowards(jumpBlend, targetJump, Time.deltaTime * 6f);
            animator.SetFloat("isJumping", jumpBlend);
        }
    }

    void FixedUpdate()
    {
        bool isGrounded = controller.isGrounded;

        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.fixedDeltaTime;

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

        Vector3 move = new Vector3(moveX * moveSpeed, velocity.y, 0f);
        controller.Move(move * Time.fixedDeltaTime);

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }

    void Jump()
    {
        bool isGrounded = controller.isGrounded;
        if (coyoteTimeCounter > 0f && jumpCooldownTimer <= 0f)
        {
            velocity.y = jumpForce;
            coyoteTimeCounter = 0f;
            jumpCooldownTimer = jumpCooldown;
            isSitting = false;
        }
    }
}