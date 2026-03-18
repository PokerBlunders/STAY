using UnityEngine;

public class LightTriggerIndicator : MonoBehaviour
{
    [Header("Light")]
    public Light targetLight;

    [Header("Trigger")]
    private Collider triggerZone;
    public string targetTag = "Player";

    [Header("Colors")]
    public Color insideColor = Color.red;
    public Color outsideColor = Color.green;

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (triggerZone == null)
            triggerZone = GetComponent<Collider>();

        SetColor(outsideColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
            SetColor(insideColor);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
            SetColor(outsideColor);
    }

    private void SetColor(Color color)
    {
        if (targetLight != null)
            targetLight.color = color;
    }
}