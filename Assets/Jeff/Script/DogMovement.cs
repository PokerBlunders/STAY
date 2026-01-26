using UnityEngine;
using System.Collections;

public class DogMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public Vector3[] positions; // 存储不同位置
    public KeyCode[] positionKeys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    [Header("QTE设置")]
    public KeyCode[] qteKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    public float qteTime = 2f;
    public int qteSequenceLength = 3; // QTE序列长度

    [Header("初始位置")]
    public Vector3 startPosition;

    [Header("QTE处理器")]
    public QTEHandler qteHandler; // 添加这个引用

    private KeyCode[] currentQTE;
    private int currentKeyIndex = 0;
    private float qteTimer = 0f;
    private bool isInQTE = false;
    private bool canReturnToStart = false;

    void Start()
    {
        startPosition = transform.position;
        Debug.Log("游戏开始! 使用方向键移动狗到不同位置");
    }

    void Update()
    {
        // 1. 移动狗到不同位置
        HandleMovement();

        // 2. 处理QTE输入
        HandleQTE();

        // 3. 处理返回初始位置
        HandleReturn();
    }

    void HandleMovement()
    {
        if (isInQTE) return; // QTE进行中不能移动

        for (int i = 0; i < positionKeys.Length; i++)
        {
            if (Input.GetKeyDown(positionKeys[i]))
            {
                // 移动到对应位置
                if (i < positions.Length)
                {
                    transform.position = positions[i];
                    Debug.Log($"狗移动到了位置 {i + 1} (按键: {positionKeys[i]})");
                    StartQTE();
                }
                break;
            }
        }
    }

    void StartQTE()
    {
        isInQTE = true;
        canReturnToStart = false;
        currentKeyIndex = 0;
        qteTimer = qteTime;

        // 生成随机的QTE序列
        currentQTE = new KeyCode[qteSequenceLength];
        for (int i = 0; i < qteSequenceLength; i++)
        {
            currentQTE[i] = qteKeys[Random.Range(0, qteKeys.Length)];
        }

        // 显示QTE提示
        string qtePrompt = "QTE开始! 按顺序输入: ";
        for (int i = 0; i < currentQTE.Length; i++)
        {
            qtePrompt += $"{currentQTE[i]} ";
        }
        qtePrompt += $"\n时间限制: {qteTime}秒";

        Debug.Log(qtePrompt);
    }

    void HandleQTE()
    {
        if (!isInQTE) return;

        // 计时
        qteTimer -= Time.deltaTime;

        // 检查超时
        if (qteTimer <= 0)
        {
            FailQTE("时间到!");
            return;
        }

        // 检查当前按键
        if (Input.GetKeyDown(currentQTE[currentKeyIndex]))
        {
            currentKeyIndex++;
            Debug.Log($"按对了! {currentKeyIndex}/{currentQTE.Length}");

            if (currentKeyIndex >= currentQTE.Length)
            {
                SuccessQTE();
                return;
            }
        }
        else
        {
            // 检查是否按了错误按键
            foreach (KeyCode key in qteKeys)
            {
                if (Input.GetKeyDown(key) && key != currentQTE[currentKeyIndex])
                {
                    FailQTE($"按错了! 应该是 {currentQTE[currentKeyIndex]}, 但你按了 {key}");
                    return;
                }
            }
        }
    }

    void SuccessQTE()
    {
        isInQTE = false;
        canReturnToStart = true;
        Debug.Log("<color=green>QTE成功完成!</color>");
        Debug.Log("按空格键返回起点");

        // 通知QTE处理器
        if (qteHandler != null)
        {
            qteHandler.OnQTESuccess();
        }

        // 成功特效
        StartCoroutine(FlashColor(Color.green));
    }

    void FailQTE(string reason)
    {
        isInQTE = false;
        canReturnToStart = false;
        Debug.LogError($"QTE失败: {reason}");
        Debug.Log("可以再次使用方向键移动狗");

        // 通知QTE处理器
        if (qteHandler != null)
        {
            qteHandler.OnQTEFailure(reason);
        }

        // 失败特效
        StartCoroutine(FlashColor(Color.red));
    }

    void HandleReturn()
    {
        if (canReturnToStart && Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = startPosition;
            canReturnToStart = false;
            Debug.Log("狗已返回起点");
            Debug.Log("使用方向键移动狗到不同位置");
        }
    }

    IEnumerator FlashColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = color;
            yield return new WaitForSeconds(0.3f);
            renderer.material.color = originalColor;
        }
    }

    // 在Unity编辑器中设置位置的辅助方法
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(startPosition, 0.5f);

        Gizmos.color = Color.green;
        if (positions != null)
        {
            foreach (Vector3 pos in positions)
            {
                Gizmos.DrawSphere(pos, 0.5f);
                Gizmos.DrawLine(startPosition, pos);
            }
        }
    }
}