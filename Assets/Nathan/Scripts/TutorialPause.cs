using UnityEngine;
using System.Collections;

public class TutorialPause : MonoBehaviour
{
    [Header("Delay Settings")]
    public float delayBeforePause = 2f;          // Seconds after entering trigger before pausing

    [Header("Input")]
    public KeyCode resumeKey = KeyCode.Mouse0;   // Default: left mouse click

    private bool isWaitingForClick = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isWaitingForClick)
        {
            StartCoroutine(DelayedPause());
        }
    }

    IEnumerator DelayedPause()
    {
        // Wait using unscaled time so the delay isn't affected by timeScale
        float elapsed = 0f;
        while (elapsed < delayBeforePause)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Pause the game
        Time.timeScale = 0f;
        isWaitingForClick = true;

        // Wait for click
        while (!Input.GetKeyDown(resumeKey))
            yield return null;

        // Resume
        Time.timeScale = 1f;
        isWaitingForClick = false;

    }
}