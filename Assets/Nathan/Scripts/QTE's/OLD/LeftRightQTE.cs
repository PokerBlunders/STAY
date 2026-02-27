using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeftRightQTE : MonoBehaviour
{
    public GameObject QTEPanel;
    public GameObject leftImage;
    public GameObject rightImage;

    public int totalClicks = 8;

    private bool active = false;
    private int clicks = 0;
    private bool nextIsLeft = true;

    void Start()
    {
        if (leftImage != null)
            leftImage.SetActive(false);
        if (rightImage != null)
            rightImage.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !active)
        {
            QTEPanel.SetActive(true);
            StartQTE();
        }
    }

    void StartQTE()
    {
        active = true;
        clicks = 0;
        nextIsLeft = Random.Range(0, 2) == 0;

        UpdateImage();

    }

    void Update()
    {
        if (!active) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (nextIsLeft)
                Correct();
            else
                Fail();
        }

        else if (Input.GetMouseButtonDown(1))
        {
            if (!nextIsLeft)
                Correct();
            else
                Fail();
        }
    }

    void Correct()
    {
        clicks++;
        nextIsLeft = !nextIsLeft;

        UpdateImage();


        if (clicks >= totalClicks)
        {
            Success();
        }
    }

    void UpdateImage()
    {
        if (leftImage != null)
            leftImage.SetActive(nextIsLeft);
        if (rightImage != null)
            rightImage.SetActive(!nextIsLeft);
    }

    void Success()
    {
        active = false;

        if (leftImage != null)
            leftImage.SetActive(false);
        if (rightImage != null)
            rightImage.SetActive(false);

        GetComponent<Collider>().enabled = false;
        QTEPanel.SetActive(false);
    }

    void Fail()
    {
        QTEPanel.SetActive(false);
        Debug.Log("LeftRightQTE: Fail");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}