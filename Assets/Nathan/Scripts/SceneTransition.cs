using UnityEngine;
using System.Collections;

public class QTESceneTransition : MonoBehaviour
{
    [Header("QTE Reference")]
    public DragQTE dragQTE;          // The DragQTE component (auto‑assigns if on same GameObject)

    [Header("Fade Settings")]
    public string sceneToLoad = "";   // Name of the scene to load after QTE success
    public float fadeDelay = 0f;      // Optional delay before fading

    void Start()
    {
        // If not assigned, try to get DragQTE on the same GameObject
        if (dragQTE == null)
            dragQTE = GetComponent<DragQTE>();

        if (dragQTE != null)
        {
            // Subscribe to the success event
            dragQTE.onSuccess.AddListener(OnQTESuccess);
        }
        else
        {
            Debug.LogError("QTESceneTransition: No DragQTE component found!");
        }
    }

    void OnQTESuccess()
    {
        StartCoroutine(DelayedFade());
    }

    IEnumerator DelayedFade()
    {
        if (fadeDelay > 0f)
            yield return new WaitForSeconds(fadeDelay);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (FadeController.Instance != null)
                FadeController.Instance.FadeToScene(sceneToLoad);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad); // fallback
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (dragQTE != null)
            dragQTE.onSuccess.RemoveListener(OnQTESuccess);
    }
}