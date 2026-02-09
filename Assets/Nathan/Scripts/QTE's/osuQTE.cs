using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class OSUQTE : MonoBehaviour
{
    public GameObject circlePrefab;
    public RectTransform canvasRoot;

    public int circlesToSpawn = 4;
    public float spawnInterval = 1f;
    public float circleLifeTime = 1.0f;

    private bool isQTERunning = false;
    private int clickedCount = 0;
    private int totalCirclesSpawned = 0;
    private bool hasFailed = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isQTERunning)
        {
            StartQTE();
        }
    }

    void StartQTE()
    {
        isQTERunning = true;
        clickedCount = 0;
        totalCirclesSpawned = 0;
        hasFailed = false;

        StartCoroutine(SpawnCircles());
    }

    IEnumerator SpawnCircles()
    {
        while (totalCirclesSpawned < circlesToSpawn && !hasFailed)
        {
            SpawnCircle();
            totalCirclesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        if (!hasFailed && clickedCount >= circlesToSpawn)
        {
            QTESuccess();
        }
    }

    void SpawnCircle()
    {
        if (hasFailed) return;

        Vector2 randomPos = new Vector2(
            Random.Range(-300, 300),
            Random.Range(-200, 200)
        );

        GameObject circle = Instantiate(circlePrefab, canvasRoot);
        circle.GetComponent<RectTransform>().anchoredPosition = randomPos;

        circle.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCircleClicked(circle);
        });
        StartCoroutine(CheckIfMissed(circle));
    }

    IEnumerator CheckIfMissed(GameObject circle)
    {
        yield return new WaitForSeconds(circleLifeTime);
        if (circle != null && !hasFailed)
        {
            QTEFail();
        }
    }

    void OnCircleClicked(GameObject circle)
    {
        if (hasFailed) return;

        clickedCount++;
        Destroy(circle);

        if (clickedCount >= circlesToSpawn && totalCirclesSpawned >= circlesToSpawn)
        {
            QTESuccess();
        }
    }

    void QTESuccess()
    {
        if (hasFailed) return;

        GetComponent<Collider>().enabled = false;
        isQTERunning = false;
    }

    void QTEFail()
    {
        if (hasFailed) return;
        hasFailed = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}