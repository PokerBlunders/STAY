using UnityEngine;
using System.Collections;

public class QTESceneTransition : MonoBehaviour
{
    [Header("QTE Reference")]
    public DragQTE dragQTE;

    [Header("Fade Settings")]
    public string sceneToLoad = "";
    public float fadeDelay = 0f;

    void Start()
    {
        if (dragQTE == null)
            dragQTE = GetComponent<DragQTE>();

        if (dragQTE != null)
        {
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
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }

    void OnDestroy()
    {
        if (dragQTE != null)
            dragQTE.onSuccess.RemoveListener(OnQTESuccess);
    }
}