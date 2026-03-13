using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FailHandler : MonoBehaviour
{
    public MovementNEW playerMovement;
    public float failAnimationDuration = 1f;
    public Animator animator;
    public GameObject shockParticles;

    private bool isFailing = false;

    public void TriggerFail()
    {
        if (isFailing) return;
        isFailing = true;

        // Lock player movement
        if (playerMovement != null)
            playerMovement.LockMovement(true);

        // Check if the dog was in crouch walk mode
        if (playerMovement != null && playerMovement.isCrouchWalk)
        {
            animator.SetBool("CrouchShock", true);
        }
        else
        {
            animator.SetBool("Shock", true);
        }

        // Activate particles
        if (shockParticles != null)
            shockParticles.SetActive(true);

        // Start the fail timer, then fade and restart
        StartCoroutine(RestartAfterFail());
    }

    private IEnumerator RestartAfterFail()
    {
        yield return new WaitForSeconds(failAnimationDuration);
        FadeController.Instance.FadeToCurrentScene();
    }
}