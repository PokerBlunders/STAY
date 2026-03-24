using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FailHandler : MonoBehaviour
{
    public MovementNEW playerMovement;
    public float failAnimationDuration = 1f;
    public Animator animator;
    public GameObject dogshockParticles;
    public GameObject screenshockParticles;

    public Trainer trainer;
    public float trainerShockDelay = 0.5f;

    [Header("Screen Shake")]
    public bool enableScreenShake = true;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.1f;

    private bool isFailing = false;
    private Vector3 originalCameraPosition;

    void Start()
    {
        if (Camera.main != null)
            originalCameraPosition = Camera.main.transform.localPosition;
    }

    public void TriggerFail()
    {
        if (isFailing) return;
        isFailing = true;

        FailCounterManager.Instance.AddFailure();

        if (playerMovement != null)
            playerMovement.LockMovement(true);

        StartCoroutine(FailSequence());
    }

    private IEnumerator FailSequence()
    {
        trainer.SetShock(true);

        yield return new WaitForSeconds(trainerShockDelay);

        // Dog shock
        if (playerMovement != null && playerMovement.isCrouchWalk)
            animator.SetBool("CrouchShock", true);
        else
            animator.SetBool("Shock", true);

        dogshockParticles.SetActive(true);
        screenshockParticles.SetActive(true);

        // Start screen shake
        if (enableScreenShake)
            StartCoroutine(ScreenShake());

        yield return new WaitForSeconds(failAnimationDuration);
        FadeController.Instance.FadeToCurrentScene();
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Vector3 originalPos = cam.transform.localPosition;

        while (elapsed < shakeDuration)
        {
            // Random offset
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = Random.Range(-shakeMagnitude, shakeMagnitude);
            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original position
        cam.transform.localPosition = originalPos;
    }
}