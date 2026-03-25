using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetPosition;

    [Header("Movement")]
    public bool useDuration = true;
    public float duration = 2f;
    public float speed = 5f;
    public bool movePosition = true;
    public bool moveRotation = true;

    [Header("Start")]
    public bool startOnAwake = true;
    public float startDelay = 0f;

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

        Vector3 finalPos = target != null ? target.position : targetPosition;
        Quaternion finalRot = target != null ? target.rotation : Quaternion.identity;

        startPos = transform.position;
        startRot = transform.rotation;

        float t = 0f;

        if (useDuration)
        {
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

        if (movePosition)
            transform.position = finalPos;
        if (moveRotation)
            transform.rotation = finalRot;

        isMoving = false;
    }
}