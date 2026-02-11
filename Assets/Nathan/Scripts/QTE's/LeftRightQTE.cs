using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeftRightQTE : MonoBehaviour
{
    public GameObject QTEPanel;
    public GameObject leftImage;
    public GameObject rightImage;

    [Header("Settings")]
    public int totalClicks = 8;     // Total clicks needed (4 left + 4 right)

    // Game state
    private bool active = false;
    private int clicks = 0;
    private bool nextIsLeft = true;

    void Start()
    {
        // Hide both images at start
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
        nextIsLeft = Random.Range(0, 2) == 0; // Randomly start with left or right

        // Show the correct image
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
        nextIsLeft = !nextIsLeft; // Switch for next click

        // Update which image is shown
        UpdateImage();


        // Check if finished
        if (clicks >= totalClicks)
        {
            Success();
        }
    }

    void UpdateImage()
    {
        // Toggle images based on which click is next
        if (leftImage != null)
            leftImage.SetActive(nextIsLeft);
        if (rightImage != null)
            rightImage.SetActive(!nextIsLeft);
    }

    void Success()
    {
        active = false;

        // Hide both images
        if (leftImage != null)
            leftImage.SetActive(false);
        if (rightImage != null)
            rightImage.SetActive(false);

        // Disable trigger so it doesn't activate again
        GetComponent<Collider>().enabled = false;
        QTEPanel.SetActive(false);
    }

    void Fail()
    {
        QTEPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}