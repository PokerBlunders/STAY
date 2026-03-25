using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class QTESceneTransition : MonoBehaviour
{
    [Header("QTE Reference")]
    public DragQTE dragQTE;

    [Header("Transition Settings")]
    public string sceneToLoad = "";
    public float delayBeforeTransition = 0f;
    public float transitionDuration = 0.5f;

    [Header("UI Image")]
    public Image transitionImage;

    private bool isTransitioning = false;

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
            transitionImage.fillAmount = 0f;
            transitionImage.gameObject.SetActive(false);
        }
    }

    void OnQTESuccess()
    {
        if (!isTransitioning && !string.IsNullOrEmpty(sceneToLoad) && transitionImage != null)
        {
            StartCoroutine(DoTransition());
        }
    }

    IEnumerator DoTransition()
    {
        isTransitioning = true;

        if (delayBeforeTransition > 0f)
            yield return new WaitForSeconds(delayBeforeTransition);

        transitionImage.gameObject.SetActive(true);
        transitionImage.fillAmount = 0f;

        float timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.unscaledDeltaTime;
            transitionImage.fillAmount = Mathf.Clamp01(timer / transitionDuration);
            yield return null;
        }
        transitionImage.fillAmount = 1f;

        SceneManager.LoadScene(sceneToLoad);

        yield return null;


        timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.unscaledDeltaTime;
            transitionImage.fillAmount = 1f - Mathf.Clamp01(timer / transitionDuration);
            yield return null;
        }
        transitionImage.fillAmount = 0f;
        transitionImage.gameObject.SetActive(false);

        isTransitioning = false;
    }

    void OnDestroy()
    {
        if (dragQTE != null)
            dragQTE.onSuccess.RemoveListener(OnQTESuccess);
    }
}