using UnityEngine;
using System.Collections;

public class QTEHandler : MonoBehaviour
{
    [Header("游戏设置")]
    public DogMovement dog;
    public float initialQteTime = 3f;
    public float minQteTime = 0.8f;
    public float difficultyIncrease = 0.9f;
    public int qtesPerLevel = 5;

    [Header("统计")]
    public int qteSuccessCount = 0;
    public int qteFailCount = 0;
    public int currentLevel = 1;
    public int totalQteAttempts = 0;

    [Header("失败说明")]
    public string[] failReasons =
    {
        "训练应该基于鼓励而非惩罚",
        "电击会造成心理创伤",
        "正向强化效果更好",
        "建立信任比强迫更重要",
        "动物也有情感和恐惧"
    };

    private float currentQteTime;
    private string currentMessage = "";

    void Start()
    {
        currentQteTime = initialQteTime;

        // 设置狗的QTE时间
        if (dog != null)
        {
            dog.qteTime = currentQteTime;
        }

        Debug.Log("QTE处理器已启动");
        UpdateStatus();
    }

    void Update()
    {
        // 监听按键事件来检测QTE状态
        // 注意：由于我们移除了statusText，现在通过其他方式检测QTE状态
        // 实际上，现在QTE状态完全由DogMovement控制，我们只需处理统计

        // 检查是否有新消息需要显示
        if (!string.IsNullOrEmpty(currentMessage))
        {
            Debug.Log(currentMessage);
            currentMessage = "";
        }
    }

    // DogMovement调用此方法通知QTE成功
    public void OnQTESuccess()
    {
        qteSuccessCount++;
        totalQteAttempts++;

        Debug.Log($"<color=green>QTE成功! 总数: {qteSuccessCount}</color>");

        // 每完成一定数量的QTE增加难度
        if (totalQteAttempts % qtesPerLevel == 0)
        {
            IncreaseDifficulty();
        }

        UpdateStatus();
    }

    // DogMovement调用此方法通知QTE失败
    public void OnQTEFailure(string reason)
    {
        qteFailCount++;
        totalQteAttempts++;

        Debug.LogError($"QTE失败! 原因: {reason}");

        // 显示随机的失败说明
        string trainingTip = failReasons[Random.Range(0, failReasons.Length)];
        Debug.Log($"<color=yellow>训练提示: {trainingTip}</color>");

        // 震动效果
        StartCoroutine(ScreenShake());

        // 每次失败都稍微增加难度
        currentQteTime = Mathf.Max(minQteTime, currentQteTime * difficultyIncrease);
        if (dog != null)
        {
            dog.qteTime = currentQteTime;
        }

        UpdateStatus();
    }

    void IncreaseDifficulty()
    {
        currentLevel++;
        currentQteTime = Mathf.Max(minQteTime, currentQteTime * difficultyIncrease);

        // 增加QTE序列长度
        if (dog != null && dog.qteSequenceLength < 6) // 最大长度限制
        {
            dog.qteSequenceLength++;
        }

        // 更新狗的设置
        if (dog != null)
        {
            dog.qteTime = currentQteTime;
        }

        // 显示难度增加提示
        Debug.Log($"<color=orange>难度增加到第 {currentLevel} 级!</color>");
        Debug.Log($"QTE时间: {currentQteTime:F1}秒");
        Debug.Log($"序列长度: {(dog != null ? dog.qteSequenceLength : 3)}");
    }

    void UpdateStatus()
    {
        // 在控制台显示统计信息
        float successRate = totalQteAttempts > 0 ?
            (float)qteSuccessCount / totalQteAttempts * 100 : 0;

        Debug.Log($"=== QTE统计 ===");
        Debug.Log($"成功: {qteSuccessCount}");
        Debug.Log($"失败: {qteFailCount}");
        Debug.Log($"成功率: {successRate:F1}%");
        Debug.Log($"当前难度: 第 {currentLevel} 级");
        Debug.Log($"QTE时间: {currentQteTime:F1}秒");
        Debug.Log($"序列长度: {(dog != null ? dog.qteSequenceLength : 3)}");
    }

    IEnumerator ScreenShake()
    {
        Vector3 originalPosition = Camera.main.transform.position;
        float shakeAmount = 0.1f;
        float shakeDuration = 0.3f;

        while (shakeDuration > 0)
        {
            Camera.main.transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalPosition;
    }

    
}