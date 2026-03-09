using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float xOffset = 0f;
    public float smoothTime = 0.1f;

    private float fixedY, fixedZ;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void FixedUpdate()
    {
        if (!target) return;

        float targetX = target.position.x + xOffset;
        Vector3 targetPosition = new Vector3(targetX, fixedY, fixedZ);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
    }
}