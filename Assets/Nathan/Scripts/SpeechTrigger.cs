using UnityEngine;
using System.Collections;

public class SpeechTrigger : MonoBehaviour
{
    [Header("Target")]
    public GameObject targetObject;          // The GameObject to activate
    public float activeDuration = 2f;         // How long to keep it active
    public string triggerTag = "Player";      // Tag that triggers the effect

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag) && targetObject != null)
        {
            StopAllCoroutines();               // Reset any ongoing timer
            StartCoroutine(ActivateAndDeactivate());
        }
    }

    private IEnumerator ActivateAndDeactivate()
    {
        targetObject.SetActive(true);
        yield return new WaitForSeconds(activeDuration);
        targetObject.SetActive(false);
    }
}