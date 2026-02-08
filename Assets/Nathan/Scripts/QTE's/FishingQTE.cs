using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FishingQTE : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform backgroundBar;     // The main bar container
    public RectTransform targetArea;        // The moving target area
    public RectTransform catchBar;          // Player-controlled bar
    public GameObject fishingPanel;         // The entire fishing UI panel
    public Image progressFill;              // Optional: Progress indicator

    [Header("Fishing Settings")]
    [Range(0.1f, 5f)] public float targetSpeed = 1f;
    [Range(0.1f, 5f)] public float catchSpeed = 2f;
    [Range(0.1f, 1f)] public float targetWidth = 0.3f;  // Width relative to background
    [Range(0.1f, 1f)] public float catchWidth = 0.2f;   // Width relative to background

    [Header("Progress Settings")]
    public float successTimeRequired = 3f;  // Seconds needed to complete
    public float failTimeAllowed = 2f;      // Max time allowed outside target
    public float progressDecayRate = 0.5f;  // How fast progress decays when outside

    // Private variables
    private bool isFishingActive = false;
    private bool isReeling = false;
    private float targetDirection = 1f;
    private float currentSuccessTime = 0f;
    private float currentFailTime = 0f;
    private float targetAreaMinY, targetAreaMaxY;
    private float catchBarMinY, catchBarMaxY;

    void Start()
    {
        // Disable UI initially
        if (fishingPanel != null)
            fishingPanel.SetActive(false);

        // Calculate movement boundaries
        CalculateBoundaries();
    }

    void CalculateBoundaries()
    {
        if (backgroundBar == null || targetArea == null || catchBar == null) return;

        // Calculate boundaries based on background bar height
        float bgHeight = backgroundBar.rect.height;
        float halfBgHeight = bgHeight * 0.5f;

        // Target area boundaries (can move within entire background)
        targetAreaMinY = -halfBgHeight + (targetArea.rect.height * 0.5f);
        targetAreaMaxY = halfBgHeight - (targetArea.rect.height * 0.5f);

        // Catch bar boundaries (can also move within entire background)
        catchBarMinY = -halfBgHeight + (catchBar.rect.height * 0.5f);
        catchBarMaxY = halfBgHeight - (catchBar.rect.height * 0.5f);
    }

    void Update()
    {
        if (!isFishingActive) return;

        // Player input for reeling
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartReeling();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopReeling();
        }

        // Move target area (random bouncing)
        MoveTargetArea();

        // Move catch bar based on reeling state
        MoveCatchBar();

        // Check if catch bar is within target area
        bool isInTarget = IsCatchInTarget();

        // Update progress
        UpdateProgress(isInTarget);

        // Update visual feedback
        UpdateVisuals(isInTarget);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFishing();
        }
    }

    void MoveTargetArea()
    {
        if (targetArea == null) return;

        // Move target up/down
        float currentY = targetArea.anchoredPosition.y;
        float newY = currentY + (targetSpeed * targetDirection * Time.deltaTime * 100f);

        // Reverse direction at boundaries
        if (newY >= targetAreaMaxY || newY <= targetAreaMinY)
        {
            targetDirection *= -1;

            // Add some randomness to speed
            targetSpeed = Random.Range(0.8f, 1.5f);

            // Optional: Randomly change direction sometimes
            if (Random.value > 0.7f)
            {
                targetDirection = Random.Range(0, 2) * 2 - 1; // -1 or 1
            }
        }

        // Clamp position
        newY = Mathf.Clamp(newY, targetAreaMinY, targetAreaMaxY);

        // Apply position
        targetArea.anchoredPosition = new Vector2(0, newY);
    }

    void MoveCatchBar()
    {
        if (catchBar == null) return;

        float currentY = catchBar.anchoredPosition.y;
        float moveAmount = catchSpeed * Time.deltaTime * 100f;

        if (isReeling)
        {
            // Move catch bar upward when reeling
            currentY += moveAmount;
        }
        else
        {
            // Move catch bar downward when not reeling
            currentY -= moveAmount;
        }

        // Clamp position
        currentY = Mathf.Clamp(currentY, catchBarMinY, catchBarMaxY);

        // Apply position
        catchBar.anchoredPosition = new Vector2(0, currentY);
    }

    void StartReeling()
    {
        isReeling = true;

    }

    void StopReeling()
    {
        isReeling = false;
    }

    bool IsCatchInTarget()
    {
        if (targetArea == null || catchBar == null) return false;

        float catchY = catchBar.anchoredPosition.y;
        float targetY = targetArea.anchoredPosition.y;
        float targetHalfHeight = targetArea.rect.height * 0.5f;
        float catchHalfHeight = catchBar.rect.height * 0.5f;

        // Check if catch bar is within target area
        return Mathf.Abs(catchY - targetY) <= (targetHalfHeight - catchHalfHeight);
    }

    void UpdateProgress(bool isInTarget)
    {
        if (isInTarget)
        {
            // Increase success timer when in target
            currentSuccessTime += Time.deltaTime;
            currentFailTime = Mathf.Max(0, currentFailTime - Time.deltaTime * 2f);
        }
        else
        {
            // Increase fail timer when outside target
            currentFailTime += Time.deltaTime;
            currentSuccessTime = Mathf.Max(0, currentSuccessTime - Time.deltaTime * progressDecayRate);
        }

        // Update progress UI
        if (progressFill != null)
        {
            float progress = currentSuccessTime / successTimeRequired;
            progressFill.fillAmount = Mathf.Clamp01(progress);
        }

        // Check win condition
        if (currentSuccessTime >= successTimeRequired)
        {
            FishingSuccess();
        }

        // Check fail condition
        if (currentFailTime >= failTimeAllowed)
        {
            FishingFail();
        }
    }

    void UpdateVisuals(bool isInTarget)
    {
        // Change catch bar color based on state
        if (catchBar != null)
        {
            Image catchImage = catchBar.GetComponent<Image>();
            if (catchImage != null)
            {
                if (isInTarget)
                {
                    catchImage.color = Color.green;
                }
                else
                {
                    catchImage.color = isReeling ? Color.yellow : Color.red;
                }
            }
        }
    }

    // Call this method to start the fishing minigame
    public void StartFishing()
    {
        isFishingActive = true;

        if (fishingPanel != null)
            fishingPanel.SetActive(true);

        // Reset values
        currentSuccessTime = 0f;
        currentFailTime = 0f;

        // Random starting positions
        if (targetArea != null)
        {
            float randomY = Random.Range(targetAreaMinY, targetAreaMaxY);
            targetArea.anchoredPosition = new Vector2(0, randomY);
        }

        if (catchBar != null)
        {
            catchBar.anchoredPosition = new Vector2(0, catchBarMinY);
        }

        Debug.Log("Fishing QTE Started! Hold Space to reel!");
    }

    void FishingSuccess()
    {
        isFishingActive = false;
        StopReeling();

        Debug.Log("Fishing Success!");

        // Optional: Wait a moment then hide UI
        StartCoroutine(CompleteFishing(true));
    }

    void FishingFail()
    {
        isFishingActive = false;
        StopReeling();

        Debug.Log("Fishing Failed!");

        // Optional: Wait a moment then restart
        StartCoroutine(CompleteFishing(false));
    }

    IEnumerator CompleteFishing(bool success)
    {
        // Wait for 1.5 seconds to show result
        yield return new WaitForSeconds(1.5f);

        // Hide UI
        if (fishingPanel != null)
            fishingPanel.SetActive(false);

        if (success)
        {
            // Continue game
            OnFishingSuccess();
        }
        else
        {
            // Restart level
            RestartLevel();
        }
    }

    void OnFishingSuccess()
    {
        // Call any other game logic here
        // Example: Give player fish item, continue dialogue, etc.
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}