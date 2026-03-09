using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FailHandler : MonoBehaviour
{
    [Header("Animation")]
    public MovementNEW playerMovement;      // Reference to player's movement script
    public float failAnimationDuration = 1f; // How long the animation plays before restart
    public Animator animator;
    public GameObject shockParticles;

    private bool isFailing = false;

    public void TriggerFail()
    {
        if (isFailing) return; // Prevent multiple fails
        isFailing = true;

        // Lock player movement
        if (playerMovement != null)
            playerMovement.LockMovement(true);

        // Play fail animation
        if (playerMovement != null)
            animator.SetBool("Shock", true);
        
        shockParticles.SetActive(true);

        // Start coroutine to restart after animation + delay
        StartCoroutine(RestartAfterFail());
    }

    private IEnumerator RestartAfterFail()
    {
        // Wait for the fail animation to finish
        yield return new WaitForSeconds(failAnimationDuration);

        FadeController.Instance.FadeToCurrentScene();

    }
}