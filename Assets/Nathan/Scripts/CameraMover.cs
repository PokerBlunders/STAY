using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // The object to move to (position and rotation)
    public Vector3 targetPosition;    // Alternative: use a custom position (if target is null)

    [Header("Movement")]
    public bool useDuration = true;   // If true, moves over a fixed time; if false, uses speed
    public float duration = 2f;       // Time to complete the movement (if useDuration = true)
    public float speed = 5f;          // Movement speed (if useDuration = false)
    public bool movePosition = true;   // Whether to move position
    public bool moveRotation = true;   // Whether to rotate towards target

    [Header("Start")]
    public bool startOnAwake = true;   // Start immediately on Awake
    public float startDelay = 0f;      // Optional delay before starting

    private Vector3 startPos;
    private Quaternion startRot;
    private bool isMoving = false;

    void Awake()
    {
        if (startOnAwake)
            StartMoving();
    }

    public void StartMoving()
    {
        if (isMoving) return;
        StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {
        isMoving = true;

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // Determine target position and rotation
        Vector3 finalPos = target != null ? target.position : targetPosition;
        Quaternion finalRot = target != null ? target.rotation : Quaternion.identity;

        startPos = transform.position;
        startRot = transform.rotation;

        float t = 0f;

        if (useDuration)
        {
            // Move over a fixed duration
            while (t < duration)
            {
                t += Time.deltaTime;
                float lerp = t / duration;
                if (movePosition)
                    transform.position = Vector3.Lerp(startPos, finalPos, lerp);
                if (moveRotation)
                    transform.rotation = Quaternion.Slerp(startRot, finalRot, lerp);
                yield return null;
            }
        }
        else
        {
            // Move at constant speed
            while (Vector3.Distance(transform.position, finalPos) > 0.01f ||
                   (moveRotation && Quaternion.Angle(transform.rotation, finalRot) > 0.1f))
            {
                if (movePosition)
                    transform.position = Vector3.MoveTowards(transform.position, finalPos, speed * Time.deltaTime);
                if (moveRotation)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRot, speed * Time.deltaTime);
                yield return null;
            }
        }

        // Snap to final values to avoid floating point errors
        if (movePosition)
            transform.position = finalPos;
        if (moveRotation)
            transform.rotation = finalRot;

        isMoving = false;
    }
}