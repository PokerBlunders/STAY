using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoSceneTransition : MonoBehaviour
{
    [Header("Settings")]
    public string targetScene = "";
    public float delay = 5f;
    public float fadeLeadTime = 2f;
    public bool useFadeEffect = true;

    void Start()
    {
        StartCoroutine(TransitionAfterDelay());
    }

    IEnumerator TransitionAfterDelay()
    {
        float waitTime = delay - fadeLeadTime;
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        if (FailCounterManager.Instance != null)
            FailCounterManager.Instance.ResetCounter();

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