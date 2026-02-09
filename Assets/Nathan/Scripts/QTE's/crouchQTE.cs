using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CrouchQTE : MonoBehaviour
{
    public GameObject staminaBarUI;
    public Slider staminaSlider;

    public float crouchDuration = 5f;
    public float staminaDrain = 15f;
    public float staminaGain = 20f;

    private bool isActive = false;
    private float currentStamina = 100f;
    private float timer = 0f;

    void Start()
    {
        if (staminaBarUI != null)
            staminaBarUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            StartQTE();
        }
    }

    void StartQTE()
    {
        isActive = true;
        currentStamina = 100f;
        timer = 0f;

        if (staminaBarUI != null)
            staminaBarUI.SetActive(true);

        if (staminaSlider != null)
            staminaSlider.value = 1f;

        Debug.Log("Crouch QTE: Spam click to keep stamina up!");
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;

        currentStamina -= staminaDrain * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            currentStamina += staminaGain;
            currentStamina = Mathf.Clamp(currentStamina, 0, 100);
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina / 100f;

            Image fillImage = staminaSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                if (currentStamina > 50) fillImage.color = Color.green;
                else if (currentStamina > 25) fillImage.color = Color.yellow;
                else fillImage.color = Color.red;
            }
        }

        if (currentStamina <= 0)
        {
            QTEFail();
            return;
        }

        if (timer >= crouchDuration)
        {
            QTESuccess();
        }
    }

    void QTESuccess()
    {
        isActive = false;

        if (staminaBarUI != null)
            staminaBarUI.SetActive(false);

        GetComponent<Collider>().enabled = false;

        Debug.Log("Crouch QTE Passed!");
    }

    void QTEFail()
    {
        isActive = false;

        Debug.Log("Crouch QTE Failed! Not enough clicks!");

        if (staminaBarUI != null)
            staminaBarUI.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}