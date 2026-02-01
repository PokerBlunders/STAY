using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QTETrigger : MonoBehaviour
{

    public GameObject qteUI;
    public Image buttonImage;
    public Sprite[] buttonSprites;


    public float qteDuration = 3f;
    public KeyCode[] possibleKeys = {KeyCode.P, KeyCode.O, KeyCode.I, KeyCode.L, KeyCode.K, KeyCode.J};

    private KeyCode requiredKey;
    private bool qteActive = false;
    private float timer;
    private bool playerInTrigger = false;


    void Update()
    {
        if (qteActive)
        {
            timer -= Time.deltaTime;

            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(requiredKey))
                {
                    QTESuccess();
                }
                else
                {
                    QTEFail();
                }
            }

            if (timer <= 0)
            {
                QTEFail();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !qteActive)
        {
            playerInTrigger = true;
            StartQTE();
        }
    }

    void StartQTE()
    {
        requiredKey = possibleKeys[Random.Range(0, possibleKeys.Length)];

        SetButtonImage(requiredKey);

        qteUI.SetActive(true);
        qteActive = true;
        timer = qteDuration;

        Debug.Log("QTE Started! Press: " + requiredKey);
    }

    void SetButtonImage(KeyCode key)
    {
        int spriteIndex = 0;

        switch (key)
        {
            case KeyCode.P: spriteIndex = 0; break;
            case KeyCode.O: spriteIndex = 1; break;
            case KeyCode.I: spriteIndex = 2; break;
            case KeyCode.L: spriteIndex = 3; break;
            case KeyCode.K: spriteIndex = 4; break;
            case KeyCode.J: spriteIndex = 5; break;
        }

        if (buttonSprites.Length > spriteIndex)
            buttonImage.sprite = buttonSprites[spriteIndex];
    }

    void QTESuccess()
    {
        qteActive = false;
        qteUI.SetActive(false);

        Debug.Log("QTE Success! Player may proceed.");

        GetComponent<Collider>().enabled = false;
    }

    void QTEFail()
    {
        qteActive = false;
        qteUI.SetActive(false);

        Debug.Log("QTE Failed! Restarting level.");
        RestartLevel();
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}