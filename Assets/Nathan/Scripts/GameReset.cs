using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoSceneTransition : MonoBehaviour
{
    [Header("Settings")]
    public string targetScene = "";        // Name of the scene to load
    public float delay = 5f;               // Total seconds before scene switch
    public float fadeLeadTime = 2f;        // How many seconds before the switch the fade should start
    public bool useFadeEffect = true;      // Use FadeController if available

    void Start()
    {
        StartCoroutine(TransitionAfterDelay());
    }

    IEnumerator TransitionAfterDelay()
    {
        // Wait for the delay minus the fade lead time (so fade starts early)
        float waitTime = delay - fadeLeadTime;
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        // Reset the failure counter before fading
        if (FailCounterManager.Instance != null)
            FailCounterManager.Instance.ResetCounter();

        // Start fade (if available) – this will take fadeDuration seconds to complete
        if (useFadeEffect && FadeController.Instance != null)
        {
            FadeController.Instance.FadeToScene(targetScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}