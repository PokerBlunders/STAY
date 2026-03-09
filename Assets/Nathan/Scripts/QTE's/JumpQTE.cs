using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpQTE : MonoBehaviour
{
    [Header("UI References")]
    public GameObject qtePanel;           // The panel containing the skill check UI
    public RectTransform needle;           // The rotating needle
    public float targetAngleMin = 240f;    // Start of safe zone (degrees)
    public float targetAngleMax = 300f;     // End of safe zone (degrees) – crosses 0°

    [Header("Settings")]
    public float rotationSpeed = 180f;     // Degrees per second
    public KeyCode triggerKey = KeyCode.Space;
    public float timeOut = 5f;             // Seconds before automatic fail (0 = no timeout)

    private bool isActive = false;
    private float currentAngle = 0f;
    private float timer = 0f;

    void Start()
    {
        if (qtePanel != null)
            qtePanel.SetActive(false);
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
        timer = 0f;
        currentAngle = 0f;
        if (qtePanel != null)
            qtePanel.SetActive(true);
        UpdateNeedle();
    }

    void Update()
    {
        if (!isActive) return;

        // Rotate needle
        currentAngle += rotationSpeed * Time.deltaTime;
        currentAngle %= 360f; // keep within 0-360
        UpdateNeedle();

        // Timeout
        if (timeOut > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= timeOut)
            {
                FailQTE();
                return;
            }
        }

        // Check for key press
        if (Input.GetKeyDown(triggerKey))
        {
            CheckSkill();
        }
    }

    void UpdateNeedle()
    {
        if (needle != null)
        {
            needle.localRotation = Quaternion.Euler(0, 0, -currentAngle);
        }
    }

    void CheckSkill()
    {
        bool success = IsAngleInTargetZone(currentAngle);
        if (success)
            SuccessQTE();
        else
            FailQTE();
    }

    bool IsAngleInTargetZone(float angle)
    {
        // Normalise target range that may cross 0°
        if (targetAngleMin <= targetAngleMax)
        {
            // Range does not cross 0°
            return angle >= targetAngleMin && angle <= targetAngleMax;
        }
        else
        {
            // Range crosses 0° (e.g., 300° to 60°)
            return angle >= targetAngleMin || angle <= targetAngleMax;
        }
    }

    void SuccessQTE()
    {
        Debug.Log("Skill check SUCCESS!");
        isActive = false;
        if (qtePanel != null)
            qtePanel.SetActive(false);
        // Optionally disable the trigger so it doesn't repeat
        GetComponent<Collider>().enabled = false;
    }

    void FailQTE()
    {
        Debug.Log("Skill check FAILED!");
        isActive = false;
        if (qtePanel != null)
            qtePanel.SetActive(false);
        // Restart the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}