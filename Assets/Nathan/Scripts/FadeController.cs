using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;

    public float fadeDuration = 0.5f;
    public float fadeInDelay = 0.3f;

    private CanvasGroup canvasGroup;
    private Coroutine currentFade;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToCurrentScene()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeOutAndLoad(SceneManager.GetActiveScene().name));
    }

    public void FadeToScene(string sceneName)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(fadeInDelay);
        canvasGroup.alpha = 1f;
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        SceneManager.LoadScene(sceneName);

        currentFade = null;
    }
}