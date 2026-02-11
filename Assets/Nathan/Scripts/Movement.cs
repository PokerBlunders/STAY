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

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = 0f;
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            moveX = 1f;
        }
        bool isGrounded = controller.isGrounded;

        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;

            if (velocity.y < 0)
                velocity.y = groundStickForce;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f && jumpCooldownTimer <= 0f)
        {
            velocity.y = jumpForce;
            coyoteTimeCounter = 0f;
            jumpCooldownTimer = jumpCooldown;
            isSitting = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            isSitting = !isSitting;
        }

        animator.SetBool("Sit", isSitting);

        velocity.y += gravity * Time.deltaTime;

        Vector3 move = new Vector3(moveX * moveSpeed, velocity.y, 0f);

        controller.Move(move * Time.deltaTime);

        float targetWalk = Mathf.Abs(moveX);
        float currentWalk = animator.GetFloat("isWalking");
        animator.SetFloat("isWalking", Mathf.MoveTowards(currentWalk, targetWalk, Time.deltaTime * 5f));

        float targetJump = isGrounded ? 0f : 1f;
        jumpBlend = Mathf.MoveTowards(jumpBlend, targetJump, Time.deltaTime * 6f);
        animator.SetFloat("isJumping", jumpBlend);

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
