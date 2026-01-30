using UnityEngine;
using UnityEngine.UI;

public class SimpleQTE : MonoBehaviour
{
    [Header("圆圈设置")]
    public RectTransform circle;           // 圆圈UI
    public float shrinkSpeed = 1.0f;       // 收缩速度
    public float minSize = 0.3f;           // 最小尺寸
    public float startSize = 1.5f;         // 起始尺寸

    [Header("时间设置")]
    public float requiredTime = 2.0f;      // 需要保持的时间
    private float currentTime = 0f;        // 当前保持时间

    [Header("反馈")]
    public Image circleImage;              // 圆圈图片组件
    public Color goodColor = Color.green;  // 正确颜色
    public Color badColor = Color.red;     // 错误颜色

    private bool isActive = false;         // QTE是否激活
    private bool mouseInside = false;      // 鼠标是否在圈内

    void Update()
    {
        if (!isActive) return;

        // 检测鼠标是否在圆圈内
        mouseInside = CheckMouseInCircle();

        if (mouseInside)
        {
            // 鼠标在圈内，增加时间并缩小圆圈
            currentTime += Time.deltaTime;

            // 缩小圆圈
            float currentScale = circle.localScale.x;
            float newScale = Mathf.Max(minSize, currentScale - Time.deltaTime * shrinkSpeed);
            circle.localScale = Vector3.one * newScale;

            // 更新颜色（根据进度）
            if (circleImage != null)
            {
                float progress = currentTime / requiredTime;
                circleImage.color = Color.Lerp(badColor, goodColor, progress);
            }

            // 检查是否成功
            if (currentTime >= requiredTime)
            {
                Debug.Log("QTE成功!");
                isActive = false;
            }
        }
        else
        {
            // 鼠标离开，失败
            Debug.Log("QTE失败!");
            isActive = false;
            if (circleImage != null)
                circleImage.color = badColor;
        }
    }

    // 检测鼠标是否在圆圈内
    bool CheckMouseInCircle()
    {
        // 获取鼠标屏幕位置
        Vector2 mousePos = Input.mousePosition;

        // 获取圆圈中心屏幕位置
        Vector2 circleScreenPos = RectTransformUtility.WorldToScreenPoint(null, circle.position);

        // 计算圆圈半径（考虑缩放）
        float radius = (circle.rect.width * circle.localScale.x) / 2f;

        // 计算距离
        float distance = Vector2.Distance(mousePos, circleScreenPos);

        return distance <= radius;
    }

    // 开始QTE
    public void StartQTE()
    {
        // 重置状态
        circle.localScale = Vector3.one * startSize;
        currentTime = 0f;
        mouseInside = false;
        isActive = true;

        // 激活圆圈
        circle.gameObject.SetActive(true);

        // 设置初始颜色
        if (circleImage != null)
            circleImage.color = badColor;
    }

    // 停止QTE
    public void StopQTE()
    {
        isActive = false;
        circle.gameObject.SetActive(false);
    }

    // 设置圆圈位置（屏幕坐标）
    public void SetPosition(Vector2 screenPosition)
    {
        // 转换屏幕坐标到UI局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            circle.parent as RectTransform,
            screenPosition,
            null,
            out Vector2 localPoint
        );

        circle.localPosition = localPoint;
    }
}