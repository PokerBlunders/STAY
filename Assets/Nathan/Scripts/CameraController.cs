using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float smoothTime = 0.15f;
    public float xOffset = 0f;

    private Vector3 velocity = Vector3.zero;
    private float fixedY;
    private float fixedZ;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (!target) return;

        float targetX = target.position.x + xOffset;

        Vector3 targetPosition = new Vector3( targetX, fixedY, fixedZ);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
