using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FishingQTE : MonoBehaviour
{
    public RectTransform backgroundBar;
    public RectTransform targetArea;
    public RectTransform catchBar;
    public GameObject fishingPanel;
    public Image progressFill;

    public float targetSpeed = 1f;
    public float catchSpeed = 2f;
    public float targetWidth = 0.3f;
    public float catchWidth = 0.2f;

    public float successTimeRequired = 3f;
    public float progressDecayRate = 0.5f;

    private bool isFishingActive = false;
    private bool isReeling = false;
    private float targetDirection = 1f;
    private float currentSuccessTime = 0f;
    private float currentFailTime = 0f;
    private float targetAreaMinY, targetAreaMaxY;
    private float catchBarMinY, catchBarMaxY;

    void Start()
    {
        if (fishingPanel != null)
            fishingPanel.SetActive(false);

        CalculateBoundaries();
    }

    void CalculateBoundaries()
    {
        if (backgroundBar == null || targetArea == null || catchBar == null) return;

        float bgHeight = backgroundBar.rect.height;
        float halfBgHeight = bgHeight * 0.5f;

        targetAreaMinY = -halfBgHeight + (targetArea.rect.height * 0.5f);
        targetAreaMaxY = halfBgHeight - (targetArea.rect.height * 0.5f);

        catchBarMinY = -halfBgHeight + (catchBar.rect.height * 0.5f);
        catchBarMaxY = halfBgHeight - (catchBar.rect.height * 0.5f);
    }

    void Update()
    {
        if (!isFishingActive) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartReeling();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopReeling();
        }

        MoveTargetArea();
        MoveCatchBar();
        bool isInTarget = IsCatchInTarget();

        UpdateProgress(isInTarget);
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

        float currentY = targetArea.anchoredPosition.y;
        float newY = currentY + (targetSpeed * targetDirection * Time.deltaTime * 100f);

        if (newY >= targetAreaMaxY || newY <= targetAreaMinY)
        {
            targetDirection *= -1;
            targetSpeed = Random.Range(0.8f, 1.5f);

            if (Random.value > 0.7f)
            {
                targetDirection = Random.Range(0, 2) * 2 - 1; // -1 or 1
            }
        }

        newY = Mathf.Clamp(newY, targetAreaMinY, targetAreaMaxY);
        targetArea.anchoredPosition = new Vector2(0, newY);
    }

    void MoveCatchBar()
    {
        if (catchBar == null) return;

        float currentY = catchBar.anchoredPosition.y;
        float moveAmount = catchSpeed * Time.deltaTime * 100f;

        if (isReeling)
        {
            currentY += moveAmount;
        }
        else
        {
            currentY -= moveAmount;
        }

        currentY = Mathf.Clamp(currentY, catchBarMinY, catchBarMaxY);
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

        return Mathf.Abs(catchY - targetY) <= (targetHalfHeight - catchHalfHeight);
    }

    void UpdateProgress(bool isInTarget)
    {
        if (isInTarget)
        {
            currentSuccessTime += Time.deltaTime;
            currentFailTime = Mathf.Max(0, currentFailTime - Time.deltaTime * 2f);
        }
        else
        {
            currentFailTime += Time.deltaTime;
            currentSuccessTime = Mathf.Max(0, currentSuccessTime - Time.deltaTime * progressDecayRate);
        }

        if (progressFill != null)
        {
            float progress = currentSuccessTime / successTimeRequired;
            progressFill.fillAmount = Mathf.Clamp01(progress);
        }

        if (currentSuccessTime >= successTimeRequired)
        {
            FishingSuccess();
        }

    }

    void UpdateVisuals(bool isInTarget)
    {
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

    public void StartFishing()
    {
        isFishingActive = true;

        if (fishingPanel != null)
            fishingPanel.SetActive(true);

        currentSuccessTime = 0f;
        currentFailTime = 0f;

        if (targetArea != null)
        {
            float randomY = Random.Range(targetAreaMinY, targetAreaMaxY);
            targetArea.anchoredPosition = new Vector2(0, randomY);
        }

        if (catchBar != null)
        {
            catchBar.anchoredPosition = new Vector2(0, catchBarMinY);
        }
    }

    void FishingSuccess()
    {
        isFishingActive = false;
        StopReeling();

        fishingPanel.SetActive(false);
    }
}