using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameText : MonoBehaviour
{
    [Header("Objects to Fade In")]
    public GameObject firstObject;      // Must have an Image component for fade; otherwise instant activation
    public GameObject secondObject;     // Same as above

    [Header("Timing")]
    public float firstDelay = 1f;       // Delay before starting first fade
    public float firstFadeDuration = 1f; // How long the first fade takes
    public float secondDelay = 2f;       // Delay after first fade completes before starting second fade
    public float secondFadeDuration = 1f;

    void Start()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // First object
        yield return new WaitForSeconds(firstDelay);
        yield return StartCoroutine(FadeInObject(firstObject, firstFadeDuration));

        // Wait between fades
        yield return new WaitForSeconds(secondDelay);

        // Second object
        yield return StartCoroutine(FadeInObject(secondObject, secondFadeDuration));
    }

    IEnumerator FadeInObject(GameObject obj, float fadeDuration)
    {
        if (obj == null) yield break;

        Image img = obj.GetComponent<Image>();
        if (img == null)
        {
            // No Image component – just activate instantly
            obj.SetActive(true);
            yield break;
        }

        // Ensure the object is active and the image starts transparent
        obj.SetActive(true);
        Color color = img.color;
        color.a = 0f;
        img.color = color;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = alpha;
            img.color = color;
            yield return null;
        }

        // Ensure fully opaque at the end
        color.a = 1f;
        img.color = color;
    }
}