using UnityEngine;
using System.Collections;

public class SceneTrigger : MonoBehaviour
{
    public string sceneToLoad;

    public GameObject player;
    private bool playerInside = false;
    private bool transitioning = false;

    public float walkForwardSpeed = 2f;
    public float walkDuration = 0.4f;

    void Update()
    {
        if (playerInside && !transitioning && Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(TransitionRoutine());
            FadeController.Instance.FadeToScene(sceneToLoad);
        }
    }

    IEnumerator TransitionRoutine()
    {
        transitioning = true;

        float timer = 0f;

        while (timer < walkDuration)
        {
            if (player != null)
            {
                player.transform.position += Vector3.forward * walkForwardSpeed * Time.deltaTime;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Press W to enter");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
