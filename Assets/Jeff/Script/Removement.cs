using UnityEngine;

public class Removement : MonoBehaviour
{

    [Header("移动设置")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -25f;
    public float groundStickForce = -5f;
    public float coyoteTime = 0.15f;

    [Header("目标点设置")]
    public GameObject[] targets = new GameObject[4];  // 4个目标点
    public int currentTargetIndex = 0;  // 当前要到达的目标点索引

    [Header("提示信息")]
    public string successMessage = "做对了! ";
    public string nextTargetMessage = "前往下一个目标";

    private CharacterController controller;
    private Vector3 velocity;
    private float coyoteTimeCounter;
    private bool hasReachedCurrentTarget = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 初始化时激活第一个目标点
        if (targets.Length > 0)
        {
            ActivateTarget(currentTargetIndex);
            Debug.Log("游戏开始! 前往第一个目标点");
        }
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

        // 按R键重置目标进度
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTargets();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查是否到达当前目标点
        if (!hasReachedCurrentTarget && currentTargetIndex < targets.Length)
        {
            GameObject currentTarget = targets[currentTargetIndex];

            if (other.gameObject == currentTarget)
            {
                hasReachedCurrentTarget = true;

                // 显示成功信息
                Debug.Log($"{successMessage} 已到达目标 {currentTargetIndex + 1}");

                // 禁用当前目标点
                currentTarget.SetActive(false);

                // 移动到下一个目标点
                currentTargetIndex++;

                if (currentTargetIndex < targets.Length)
                {
                    // 激活下一个目标点
                    ActivateTarget(currentTargetIndex);
                    Debug.Log($"{nextTargetMessage} {currentTargetIndex + 1}");
                }
                else
                {
                    // 所有目标都已完成
                    Debug.Log("恭喜! 你已到达所有目标点!");
                }
            }
        }
    }

    // 激活指定索引的目标点
    void ActivateTarget(int index)
    {
        if (index >= 0 && index < targets.Length)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    // 激活当前目标点，隐藏其他目标点
                    if (i == index)
                    {
                        targets[i].SetActive(true);
                        hasReachedCurrentTarget = false;
                    }
                    else
                    {
                        targets[i].SetActive(false);
                    }
                }
            }
        }
    }

    // 重置所有目标点
    void ResetTargets()
    {
        currentTargetIndex = 0;
        hasReachedCurrentTarget = false;

        // 激活第一个目标点，隐藏其他
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                if (i == 0)
                {
                    targets[i].SetActive(true);
                }
                else
                {
                    targets[i].SetActive(false);
                }
            }
        }

        Debug.Log("目标点已重置! 前往第一个目标点");
    }

    // 在Unity编辑器中显示目标点位置
    void OnDrawGizmosSelected()
    {
        if (targets != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    Gizmos.DrawWireSphere(targets[i].transform.position, 1f);
                    Gizmos.DrawIcon(targets[i].transform.position, "Target.png", true);

                    // 绘制连线
                    if (i > 0 && targets[i - 1] != null)
                    {
                        Gizmos.DrawLine(targets[i - 1].transform.position, targets[i].transform.position);
                    }
                }
            }
        }
    }
}

