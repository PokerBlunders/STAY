using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;   // Required for coroutines

public class DragQTE : MonoBehaviour
{
    [Header("UI References")]
    public GameObject qtePanel;
    public Slider dragSlider;

    [Header("Settings")]
    public float timeLimit = 5f;
    public float targetValue = 0f;
    public bool failOnRelease = true;
    public FailHandler failHandler;

    [Header("Player")]
    public MovementNEW playerMovement;

    [Header("Animation on Success")]
    public MovementNEW.AnimationType successAnimation = MovementNEW.AnimationType.Sit;
    public float animationDuration = 2f;
    public float postAnimationLock = 0.5f;   // extra lock time after animation ends

    [Header("Next QTE")]
    public GameObject nextQTE;
    public bool StandUpAtEnd;   // Checkbox to control final standing

    private bool isActive = false;
    private float timer = 0f;
    private bool hasSucceeded = false;

    void Start()
    {
        if (qtePanel != null)
            qtePanel.SetActive(false);

        if (dragSlider != null)
        {
            dragSlider.value = 1f;
            dragSlider.interactable = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            StartQTE();
        }
    }

    void StartQTE()
    {
        isActive = true;
        hasSucceeded = false;
        timer = 0f;

        if (qtePanel != null)
            qtePanel.SetActive(true);
        if (playerMovement != null)
            playerMovement.LockMovement(true);

        if (dragSlider != null)
        {
            dragSlider.value = 1f;
            dragSlider.interactable = true;
            dragSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void Update()
    {
        if (!isActive || hasSucceeded) return;

        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            FailQTE();
        }
    }

    void OnSliderValueChanged(float value)
    {
        if (!isActive || hasSucceeded) return;

        if (Mathf.Approximately(value, targetValue))
        {
            SuccessQTE();
        }
    }

    public void OnHandlePointerUp()
    {
        if (!isActive || hasSucceeded) return;

        if (failOnRelease && !Mathf.Approximately(dragSlider.value, targetValue))
        {
            FailQTE();
        }
    }

    void SuccessQTE()
    {
        Debug.Log("Drag QTE SUCCESS!");
        hasSucceeded = true;

        // Play the selected animation, passing the standUpAtEnd flag
        if (playerMovement != null)
            playerMovement.PlayAnimation(successAnimation, animationDuration, postAnimationLock, StandUpAtEnd);

        // Start coroutine to enable next QTE after animation + post‑lock
        if (nextQTE != null)
        {
            StartCoroutine(EnableNextQTEDelayed());
        }

        // Clean up UI
        if (dragSlider != null)
        {
            dragSlider.interactable = false;
            dragSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        if (qtePanel != null)
            qtePanel.SetActive(false);

        // Disable this trigger so it doesn't activate again
        GetComponent<Collider>().enabled = false;
    }

    IEnumerator EnableNextQTEDelayed()
    {
        float totalDelay = animationDuration;
        yield return new WaitForSeconds(totalDelay);
        if (nextQTE != null)
            nextQTE.SetActive(true);
    }

    void FailQTE()
    {
        if (!isActive) return;
        Debug.Log("Drag QTE FAILED!");

        // Clean up UI and unlock player (the fail handler may also lock/unlock, but we do it here for safety)
        if (dragSlider != null)
        {
            dragSlider.interactable = false;
            dragSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        if (qtePanel != null)
            qtePanel.SetActive(false);
        if (playerMovement != null)
            playerMovement.LockMovement(false);

        failHandler.TriggerFail();
    }
}