using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SimpleTimedChallenge : MonoBehaviour
{
    [Header("QTE Reference")]
    public MonoBehaviour qteScript;

    private bool isActive = false;
    private float startTime = 0f;

    void Start()
    {
        if (qteScript != null)
        {
            var eventField = qteScript.GetType().GetField("onSuccess");
            if (eventField != null)
            {
                var unityEvent = eventField.GetValue(qteScript) as UnityEvent;
                if (unityEvent != null)
                    unityEvent.AddListener(OnQTESuccess);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            StartTimer();
        }
    }

    void StartTimer()
    {
        isActive = true;
        startTime = Time.time;
    }

    void OnQTESuccess()
    {
        if (!isActive) return;
        float elapsed = Time.time - startTime;
        isActive = false;
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"'{sceneName}': {elapsed:F2} seconds.");
    }

    void OnDestroy()
    {
        if (qteScript != null)
        {
            var eventField = qteScript.GetType().GetField("onSuccess");
            if (eventField != null)
            {
                var unityEvent = eventField.GetValue(qteScript) as UnityEvent;
                if (unityEvent != null)
                    unityEvent.RemoveListener(OnQTESuccess);
            }
        }
    }
}