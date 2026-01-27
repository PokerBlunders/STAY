using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrainerMovement : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float waitAtPointTime = 1.5f;

    public float timeOutsideTriggerBeforeReset = 2f;

    private int currentPointIndex = 0;
    private bool isWaiting = false;

    private bool playerInsideTrigger = true;
    private float outsideTimer = 0f;

    void Start()
    {
        if (patrolPoints.Length > 0)
            transform.position = patrolPoints[0].position;
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
        yield return new WaitForSeconds(waitAtPointTime);

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        isWaiting = false;
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
}
