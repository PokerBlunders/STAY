using UnityEngine;
using TMPro;
using System.Collections;

public class LRQTE : MonoBehaviour
{
    public TMP_Text promptText;

    public float totalTime = 5f;
    public float timeLimit = 1f;

    private bool needLeft;
    private float timer = 0f;

    void Start()
    {
        StartCoroutine(QTERoutine());
    }

    IEnumerator QTERoutine()
    {
        timer = 0f;

        while (timer < totalTime)
        {
            // ⭐每一轮随机决定左右键
            needLeft = Random.value > 0.5f;

            // 显示提示
            promptText.text = needLeft
                ? "LEFT"
                : "RIGHT";

            float waitTime = 0f;
            bool pressedCorrect = false;

            // 等待输入
            while (waitTime < timeLimit)
            {
                // 左键
                if (Input.GetMouseButtonDown(0))
                {
                    if (needLeft)
                    {
                        pressedCorrect = true;
                        break;
                    }
                    else
                    {
                        Fail();
                        yield break;
                    }
                }

                // 右键
                if (Input.GetMouseButtonDown(1))
                {
                    if (!needLeft)
                    {
                        pressedCorrect = true;
                        break;
                    }
                    else
                    {
                        Fail();
                        yield break;
                    }
                }

                waitTime += Time.deltaTime;
                yield return null;
            }

            // 超时失败
            if (!pressedCorrect)
            {
                Fail();
                yield break;
            }

            // ⭐清除输入残留
            yield return null;

            timer += waitTime;
        }

        Success();
    }

    void Fail()
    {
        promptText.text = "hahaha loser";
        
        StopAllCoroutines();
    }

    void Success()
    {
        promptText.text = "SUCCESS!";
      
    }
}
