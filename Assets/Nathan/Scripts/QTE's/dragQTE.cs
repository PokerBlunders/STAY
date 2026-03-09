using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DragQTE : MonoBehaviour
{
    [Header("UI References")]
    public GameObject qtePanel;           // The panel containing the slider
    public Slider dragSlider;              // The slider itself

    [Header("Settings")]
    public float timeLimit = 5f;           // Seconds before automatic failure
    public float targetValue = 0f;         // 0 = bottom, 1 = top (we want bottom)
    public bool failOnRelease = true;      // Fail if player releases before reaching target

    [Header("Player")]
    public MovementNEW playerMovement;      // Reference to player’s movement script (to lock controls)

    private bool isActive = false;
    private float timer = 0f;
    private bool hasSucceeded = false;

    void Start()
    {
        if (qtePanel != null)
            qtePanel.SetActive(false);

        // Ensure slider starts at the top (value = 1)
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

        // Show UI and lock player movement
        if (qtePanel != null)
            qtePanel.SetActive(true);
        if (playerMovement != null)
            playerMovement.LockMovement(true);

        // Reset slider and enable it
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

        // Check if the slider has reached the target (bottom)
        if (Mathf.Approximately(value, targetValue))
        {
            SuccessQTE();
        }
    }

    // Called by the EventTrigger on the handle when the mouse button is released
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

        // Clean up
        if (dragSlider != null)
        {
            dragSlider.interactable = false;
            dragSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        if (qtePanel != null)
            qtePanel.SetActive(false);
        if (playerMovement != null)
            playerMovement.LockMovement(false);

        // Disable this trigger so it doesn't activate again
        GetComponent<Collider>().enabled = false;
    }

    void FailQTE()
    {
        if (!isActive) return;
        Debug.Log("Drag QTE FAILED!");

        // Clean up and restart level
        if (dragSlider != null)
        {
            dragSlider.interactable = false;
            dragSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        if (qtePanel != null)
            qtePanel.SetActive(false);
        if (playerMovement != null)
            playerMovement.LockMovement(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}