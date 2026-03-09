using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float xOffset = 0f;
    public float smoothTime = 0.1f;

    [Header("Aspect Ratio Lock")]
    public bool lockAspectRatio = true;
    public float targetAspect = 16f / 9f;

    private float fixedY, fixedZ;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
        cam = GetComponent<Camera>();

        if (lockAspectRatio && cam != null)
        {
            ApplyAspectRatio();
        }
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void FixedUpdate()
    {
        if (!target) return;

        float targetX = target.position.x + xOffset;
        Vector3 targetPosition = new Vector3(targetX, fixedY, fixedZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
    }

    void Update()
    {
        if (lockAspectRatio && cam != null)
        {
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                ApplyAspectRatio();
            }
        }
    }

    void ApplyAspectRatio()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }
}