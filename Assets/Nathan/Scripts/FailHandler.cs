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

        if (playerMovement != null)
            playerMovement.LockMovement(true);

        if (playerMovement != null)
            animator.SetBool("Shock", true);
        
        shockParticles.SetActive(true);

        StartCoroutine(RestartAfterFail());
    }

    private IEnumerator RestartAfterFail()
    {
        yield return new WaitForSeconds(failAnimationDuration);

        FadeController.Instance.FadeToCurrentScene();

    }
}