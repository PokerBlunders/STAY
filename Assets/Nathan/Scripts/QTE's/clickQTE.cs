using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class clickQTE : MonoBehaviour
{
    public enum QTEType { Crouch, LeftRight }

    [Header("UI References")]
    public GameObject qtePanel;
    public GameObject buttonPrefab;
    public RectTransform[] spawnPositions;

    [Header("Settings")]
    public QTEType type = QTEType.Crouch;
    public float timePerStep = 2f;
    public float crouchMoveSpeed = 1f;
    public float leftRightMoveSpeed = 1f;

    [Header("Left/Right Dodge Settings")]
    public float dodgeInterval = 0.5f;
    public bool startWithLeft = true;

    [Header("Player")]
    public MovementNEW playerMovement;

    [Header("Finish Zone")]
    public Collider finishZone;

    [Header("Failure")]
    public FailHandler failHandler;

    private GameObject currentButton;
    private int currentIndex = 0;
    private float timer = 0f;
    private bool isActive = false;
    private bool sequenceCompleted = false;

    void Start()
    {
        if (qtePanel != null)
            qtePanel.SetActive(false);

        if (playerMovement == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerMovement = player.GetComponent<MovementNEW>();
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
        currentIndex = 0;
        sequenceCompleted = false;

        if (playerMovement != null)
        {
            if (type == QTEType.Crouch)
                playerMovement.StartCrouchSequence(crouchMoveSpeed);
            else
                playerMovement.StartLeftRightSequence(leftRightMoveSpeed, startWithLeft, dodgeInterval, spawnPositions.Length);
        }

        if (qtePanel != null)
            qtePanel.SetActive(true);

        SpawnButtonAtCurrentPosition();
        timer = timePerStep;
    }

    void Update()
    {
        if (!isActive) return;

        if (!sequenceCompleted)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                FailQTE();
                return;
            }
        }

        if (sequenceCompleted && finishZone != null && playerMovement != null)
        {
            if (finishZone.bounds.Contains(playerMovement.transform.position))
            {
                SuccessQTE();
            }
        }
    }

    void SpawnButtonAtCurrentPosition()
    {
        if (currentButton != null)
            Destroy(currentButton);

        if (buttonPrefab != null && spawnPositions.Length > 0 && currentIndex < spawnPositions.Length)
        {
            currentButton = Instantiate(buttonPrefab, spawnPositions[currentIndex].position, Quaternion.identity, spawnPositions[currentIndex].parent);
            Button btn = currentButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnButtonClicked);
            }
        }
    }

    void OnButtonClicked()
    {
        if (!isActive) return;

        currentIndex++;

        if (currentIndex >= spawnPositions.Length)
        {
            sequenceCompleted = true;
            if (currentButton != null)
                Destroy(currentButton);
            if (qtePanel != null)
                qtePanel.SetActive(false);
        }
        else
        {
            SpawnButtonAtCurrentPosition();
            timer = timePerStep;
        }
    }
    void SuccessQTE()
    {
        isActive = false;

        if (qtePanel != null)
            qtePanel.SetActive(false);
        if (currentButton != null)
            Destroy(currentButton);

        if (playerMovement != null)
            playerMovement.StopSequence();
        if (type == QTEType.Crouch)
        {
            playerMovement.ResetCrouchWalk();
        }

        GetComponent<Collider>().enabled = false;
    }

    void FailQTE()
    {
        if (!isActive) return;
        isActive = false;

        if (failHandler != null)
            failHandler.TriggerFail();

        if (qtePanel != null)
            qtePanel.SetActive(false);
        if (currentButton != null)
            Destroy(currentButton);

        if (playerMovement != null)
            playerMovement.StopSequence();
    }
}