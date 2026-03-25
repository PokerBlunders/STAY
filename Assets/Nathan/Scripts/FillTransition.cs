using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class FillTransition : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage;

    [Header("Settings")]
    public float transitionDuration = 0.5f;
    public FillMethod fillMethod = FillMethod.Horizontal;

    public static FillTransition Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = fillMethod;
        fillImage.fillAmount = 0f;
        fillImage.gameObject.SetActive(false);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(DoTransition(sceneName));
    }

    private IEnumerator DoTransition(string sceneName)
    {

        fillImage.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            fillImage.fillAmount = t;
            elapsed += Time.deltaTime;
            yield return null;
        }
        fillImage.fillAmount = 1f;

        SceneManager.LoadScene(sceneName);

        yield return null;

        elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            fillImage.fillAmount = 1f - t;
            elapsed += Time.deltaTime;
            yield return null;
        }
        fillImage.fillAmount = 0f;

        fillImage.gameObject.SetActive(false);
    }
}