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
        trainer.SetShock(true);

        yield return new WaitForSeconds(trainerShockDelay);

      
       if (playerMovement.isCrouchWalk)
            animator.SetBool("CrouchShock", true);
        else
            animator.SetBool("Shock", true);
 
        shockParticles.SetActive(true);

        yield return new WaitForSeconds(failAnimationDuration);
        FadeController.Instance.FadeToCurrentScene();
    }
}