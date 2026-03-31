using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float duration = 5f;              // Countdown time in seconds
    public Image fillImage;                  // Image to update (must be set to Filled type)
    public bool startOnTrigger = true;       // If true, starts when player enters trigger

    [Header("Failure")]
    public FailHandler failHandler;          // Reference to fail handler (will find if null)

    private bool isRunning = false;
    private float currentTime = 0f;

    void Start()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (startOnTrigger && other.CompareTag("Player") && !isRunning)
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        if (isRunning) return;
        isRunning = true;
        currentTime = duration;
        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
            fillImage.gameObject.SetActive(true);
        }
        StartCoroutine(Countdown());
    }

    public void StopTimer(bool triggerFailure = false)
    {
        if (!isRunning) return;
        StopCoroutine(Countdown());
        isRunning = false;
        if (fillImage != null)
            fillImage.gameObject.SetActive(false);

        if (triggerFailure && failHandler != null)
            failHandler.TriggerFail();
    }

    private IEnumerator Countdown()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (fillImage != null)
                fillImage.fillAmount = currentTime / duration;
            yield return null;
        }

        // Time's up
        isRunning = false;
        if (fillImage != null)
            fillImage.gameObject.SetActive(false);

        if (failHandler != null)
            failHandler.TriggerFail();
    }
}