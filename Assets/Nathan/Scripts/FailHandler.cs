using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FailHandler : MonoBehaviour
{
    public MovementNEW playerMovement;
    public float failAnimationDuration = 1f;
    public Animator animator;
    public GameObject shockParticles;

    public Trainer trainer;
    public float trainerShockDelay = 0.5f;

    private bool isFailing = false;

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

      
       if (playerMovement.isCrouchWalk)
            animator.SetBool("CrouchShock", true);
        else
            animator.SetBool("Shock", true);
 
        shockParticles.SetActive(true);

        yield return new WaitForSeconds(failAnimationDuration);
        FadeController.Instance.FadeToCurrentScene();
    }
}