using UnityEngine;

public class LightTriggerIndicator : MonoBehaviour
{
    [Header("Light")]
    public Light targetLight;               // The light to change (if not assigned, tries to find on this object)

    [Header("Trigger")]
    private Collider triggerZone;             // The trigger collider to monitor (if not assigned, tries to find on this object)
    public string targetTag = "Player";      // Tag of the object that triggers the change

    [Header("Colors")]
    public Color insideColor = Color.red;
    public Color outsideColor = Color.green;

    private void Start()
    {
        // Auto‑assign light if not set
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        // Auto‑assign trigger collider if not set
        if (triggerZone == null)
            triggerZone = GetComponent<Collider>();

        // Set initial color to outside
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