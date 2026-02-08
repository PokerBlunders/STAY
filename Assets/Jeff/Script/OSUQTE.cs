using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OSUQTE : MonoBehaviour
{
    public GameObject circlePrefab;
    public RectTransform canvasRoot;

    public int maxPerGroup = 4;

    public float spawnInterval = 1f;   
    public float circleLifeTime = 0.5f; 

    private int spawnedCount = 0;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (spawnedCount < maxPerGroup)
        {
            SpawnCircle();

            spawnedCount++;

         
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("good");
    }

    void SpawnCircle()
    {
        
        Vector2 randomPos = new Vector2(
            Random.Range(-300, 300),
            Random.Range(-200, 200)
        );

        
        GameObject circle = Instantiate(circlePrefab, canvasRoot);
        circle.GetComponent<RectTransform>().anchoredPosition = randomPos;

        // if hit 
        circle.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Hit!");
            Destroy(circle);
        });

        //gone in 1s
        Destroy(circle, circleLifeTime);
    }
}
