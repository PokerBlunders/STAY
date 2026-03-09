using UnityEngine;
using UnityEngine.SceneManagement;

public class jumpQTE : MonoBehaviour
{
    [Header("UI References")]
    public GameObject qtePanel;
    public RectTransform needle;
    public float targetAngleMin = 240f;
    public float targetAngleMax = 300f;

    [Header("Settings")]
    public float rotationSpeed = 180f;
    public KeyCode triggerKey = KeyCode.Space;
    public float timeOut = 5f;
    public float lungeDuration = 0.5f;
    public float jumpForceOverride = 0f;
    public float airSpeedMultiplier = 1f;

    [Header("Player Link")]
    public MovementNEW playerMovement;

    private bool isActive = false;
    private float currentAngle = 0f;
    private float timer = 0f;

    void Start()
    {
        if (qtePanel != null)
            qtePanel.SetActive(false);

        if (playerMovement == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerMovement = player.GetComponent<MovementNEW>();
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
        timer = 0f;
        currentAngle = 0f;

        if (playerMovement != null)
            playerMovement.LockMovement(true);

        if (qtePanel != null)
            qtePanel.SetActive(true);
        UpdateNeedle();
    }

    void Update()
    {
        if (!isActive) return;

        currentAngle += rotationSpeed * Time.deltaTime;
        currentAngle %= 360f;
        UpdateNeedle();

        if (timeOut > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= timeOut)
            {
                FailQTE();
                return;
            }
        }

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
        if (targetAngleMin <= targetAngleMax)
            return angle >= targetAngleMin && angle <= targetAngleMax;
        else
            return angle >= targetAngleMin || angle <= targetAngleMax;
    }

    void SuccessQTE()
    {
        if (playerMovement != null)
            playerMovement.PerformJumpLunge(lungeDuration, jumpForceOverride, airSpeedMultiplier);

        isActive = false;
        if (qtePanel != null)
            qtePanel.SetActive(false);

        GetComponent<Collider>().enabled = false;
    }

    void FailQTE()
    {
        isActive = false;
        if (qtePanel != null)
            qtePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.LockMovement(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}