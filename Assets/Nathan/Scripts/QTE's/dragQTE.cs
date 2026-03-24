using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragQTE : MonoBehaviour
{
    [Header("UI")]
    public GameObject qtePanel;
    public Slider dragSlider;
    public Image timerImage;               // <-- NEW: assign a filled Image (e.g., a progress ring)

    [Header("Settings")]
    public float timeLimit = 5f;
    public float targetValue = 0f;
    public bool failOnRelease = true;
    public FailHandler failHandler;

    [Header("Player")]
    public MovementNEW playerMovement;

    [Header("Animation")]
    public MovementNEW.AnimationType successAnimation = MovementNEW.AnimationType.Sit;
    public float animationDuration = 2f;
    public float postAnimationLock = 0.5f;

    [Header("Next QTE")]
    public GameObject nextQTE;
    public bool StandUpAtEnd;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onSuccess;

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

        // Ensure timer image is hidden initially
        if (timerImage != null)
            timerImage.gameObject.SetActive(false);
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

        // Reset timer visual
        if (timerImage != null)
        {
            timerImage.fillAmount = 1f;
            timerImage.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!isActive || hasSucceeded) return;

        timer += Time.deltaTime;

        // Update timer image
        if (timerImage != null)
        {
            timerImage.fillAmount = 1f - (timer / timeLimit);
        }

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
        hasSucceeded = true;

        if (playerMovement != null)
            playerMovement.PlayAnimation(successAnimation, animationDuration, postAnimationLock, StandUpAtEnd);

        if (nextQTE != null)
        {
            // Pass both animation duration and post lock to the coroutine
            StartCoroutine(EnableNextQTEDelayed(animationDuration, postAnimationLock));
        }

        if (dragSlider != null)
        {
            dragSlider.interactable = false;
            dragSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        if (qtePanel != null)
            qtePanel.SetActive(false);

        // Hide timer image
        if (timerImage != null)
            timerImage.gameObject.SetActive(false);

        onSuccess?.Invoke();

        GetComponent<Collider>().enabled = false;
    }

    IEnumerator EnableNextQTEDelayed(float animDuration, float postLock)
    {
        float totalDelay = animDuration + postLock;
        yield return new WaitForSeconds(totalDelay);
        if (nextQTE != null)
            nextQTE.SetActive(true);
    }

    void FailQTE()
    {
        if (!isActive) return;

        if (dragSlider != null)
        {
            dragSlider.interactable = false;
            dragSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        if (qtePanel != null)
            qtePanel.SetActive(false);
        if (timerImage != null)
            timerImage.gameObject.SetActive(false);
        if (playerMovement != null)
            playerMovement.LockMovement(false);

        failHandler.TriggerFail();
    }
}