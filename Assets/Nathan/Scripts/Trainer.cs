using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BackgroundCharacter : MonoBehaviour
{
    [Header("Movement")]
    public Transform waypoint;
    public float moveSpeed = 2f;
    public float stopDistance = 0.1f;
    public float moveStartDelay = 1f;

    [Header("Animator")]
    public Animator animator;
    public string walkBool = "walk";
    public string arriveBool = "arrive";
    public string shockBool = "shock";

    private bool isMoving = false;
    private Coroutine moveCoroutine;

    public void SetWalk(bool value)
    {
        if (animator != null) animator.SetBool(walkBool, value);
    }

    public void SetArrive(bool value)
    {
        if (animator != null) animator.SetBool(arriveBool, value);
    }

    public void SetShock(bool value)
    {
        if (animator != null) animator.SetBool(shockBool, value);
    }

    public void StartWalking()
    {
        if (waypoint == null)
        {
            Debug.LogWarning("No waypoint assigned!");
            return;
        }
        if (isMoving) return;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        SetArrive(false);
        moveCoroutine = StartCoroutine(DelayedMove());
    }

    private IEnumerator DelayedMove()
    {
        isMoving = true;

        SetWalk(true);

        if (moveStartDelay > 0f)
            yield return new WaitForSeconds(moveStartDelay);

        while (Vector3.Distance(transform.position, waypoint.position) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = waypoint.position;

        SetWalk(false);
        SetArrive(true);

        isMoving = false;
        moveCoroutine = null;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartWalking();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetShock(true);
        }
    }
}