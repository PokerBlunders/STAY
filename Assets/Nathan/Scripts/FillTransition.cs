using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class FillTransition : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage;               // Must be type "Filled" (e.g., a black panel)

    [Header("Settings")]
    public float transitionDuration = 0.5f;
    public FillMethod fillMethod = FillMethod.Horizontal;   // Horizontal, Vertical, etc.

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

        if (fillImage == null)
        {
            Debug.LogError("FillTransition: No fillImage assigned!");
            return;
        }

        // Ensure the image is set up correctly
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = fillMethod;
        fillImage.fillAmount = 0f;
        fillImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Call this to start the transition to a new scene.
    /// </summary>
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(DoTransition(sceneName));
    }

    private IEnumerator DoTransition(string sceneName)
    {
        // Activate the image and fill it to cover the screen
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

        // Load the new scene while the screen is fully covered
        SceneManager.LoadScene(sceneName);

        // Wait one frame to let the new scene start
        yield return null;

        // Un‑fill to reveal the new scene
        elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            fillImage.fillAmount = 1f - t;
            elapsed += Time.deltaTime;
            yield return null;
        }
        fillImage.fillAmount = 0f;

        // Hide the image
        fillImage.gameObject.SetActive(false);
    }
}