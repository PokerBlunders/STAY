using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -25f;

    public float groundStickForce = -5f;
    public float coyoteTime = 0.15f;

    private CharacterController controller;
    private Vector3 velocity;
    private float coyoteTimeCounter;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        if (controller.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            if (velocity.y < 0)
                velocity.y = groundStickForce;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            velocity.y = jumpForce;
            coyoteTimeCounter = 0f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 move = new Vector3(
            moveX * moveSpeed,
            velocity.y,
            0f
        );

        controller.Move(move * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
