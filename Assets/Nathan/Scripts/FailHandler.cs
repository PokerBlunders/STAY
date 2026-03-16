using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FailHandler : MonoBehaviour
{
    public MovementNEW playerMovement;
    public float failAnimationDuration = 1f;   // Duration of dog's shock animation (after trainer)
    public Animator animator;                   // Dog's animator
    public GameObject shockParticles;

    public Trainer trainer;                      // Reference to the trainer character
    public float trainerShockDelay = 0.5f;       // Time to wait after trainer animation before dog shock

    private bool isFailing = false;

    public void TriggerFail()
    {
        if (isFailing) return;
        isFailing = true;

        // Lock player movement immediately
        if (playerMovement != null)
            playerMovement.LockMovement(true);

        // Start the coordinated fail sequence
        StartCoroutine(FailSequence());
    }

    private IEnumerator FailSequence()
    {
        // 1. Trigger trainer's shock animation (assumes Trainer has a public method)
        if (trainer != null)
            trainer.SetShock(true);   // You'll need to implement this in your Trainer script

        // 2. Wait for the trainer's shock to play (or a fixed delay)
        yield return new WaitForSeconds(trainerShockDelay);

        // 3. Now trigger dog's shock animation and particles
        if (animator != null)
        {
            if (playerMovement != null && playerMovement.isCrouchWalk)
                animator.SetBool("CrouchShock", true);
            else
                animator.SetBool("Shock", true);
        }

        if (shockParticles != null)
            shockParticles.SetActive(true);

        // 4. Wait for the dog's shock duration, then fade and restart
        yield return new WaitForSeconds(failAnimationDuration);
        FadeController.Instance.FadeToCurrentScene();
    }
}