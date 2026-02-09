using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrainerMovement : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float[] waitTimesAtPoints;

    public float moveSpeed = 2f;
    public float defaultWaitTime = 1.5f;

    public float timeOutsideTriggerBeforeReset = 2f;

    private int currentPointIndex = 0;
    private bool isWaiting = false;

    private bool playerInsideTrigger = true;
    private float outsideTimer = 0f;

    void Start()
    {
        InitializeWaitTimes();

        if (patrolPoints.Length > 0)
            transform.position = patrolPoints[0].position;
    }

    void InitializeWaitTimes()
    {
        if (waitTimesAtPoints == null || waitTimesAtPoints.Length != patrolPoints.Length)
        {
            float[] newWaitTimes = new float[patrolPoints.Length];

            if (waitTimesAtPoints != null && waitTimesAtPoints.Length > 0)
            {
                for (int i = 0; i < Mathf.Min(waitTimesAtPoints.Length, newWaitTimes.Length); i++)
                {
                    newWaitTimes[i] = waitTimesAtPoints[i];
                }

                for (int i = waitTimesAtPoints.Length; i < newWaitTimes.Length; i++)
                {
                    newWaitTimes[i] = defaultWaitTime;
                }
            }
            else
            {
                for (int i = 0; i < newWaitTimes.Length; i++)
                {
                    newWaitTimes[i] = defaultWaitTime;
                }
            }

            waitTimesAtPoints = newWaitTimes;
        }
    }

    void Update()
    {
        HandlePatrol();
        HandlePlayerDetection();
    }

    void HandlePatrol()
    {
        if (patrolPoints.Length == 0 || isWaiting)
            return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;

        float waitTime = GetWaitTimeForCurrentPoint();
        yield return new WaitForSeconds(waitTime);

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        isWaiting = false;
    }

    float GetWaitTimeForCurrentPoint()
    {
        if (waitTimesAtPoints != null && currentPointIndex < waitTimesAtPoints.Length)
        {
            return waitTimesAtPoints[currentPointIndex];
        }
        return defaultWaitTime;
    }

    void HandlePlayerDetection()
    {
        if (!playerInsideTrigger)
        {
            outsideTimer += Time.deltaTime;

            if (outsideTimer >= timeOutsideTriggerBeforeReset)
            {
                ResetGame();
            }
        }
        else
        {
            outsideTimer = 0f;
        }
    }

    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
        }
    }

    // Optional: Visualize wait times in editor
    void OnDrawGizmosSelected()
    {
        if (patrolPoints == null) return;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            // Draw a sphere at each patrol point
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);

            // Draw line to next point
            if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
            else if (i == patrolPoints.Length - 1 && patrolPoints[0] != null)
            {
                // Connect last to first for loop
                Gizmos.color = Color.white;
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
            }
        }
    }
}