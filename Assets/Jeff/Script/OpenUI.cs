using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OpenUI : MonoBehaviour
{
    public GameObject menu;
    public GameObject setting;
    public UnityEvent PausedAll;
    public UnityEvent ResumAll;
    bool isPaused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menu.SetActive(false);
        setting.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressedToPlay();
        }
    }
    public void PressedToPlay()
    {
        menu.SetActive(!menu.activeSelf);
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            PausedAll.Invoke();
        }
        else
        {
            Time.timeScale = 1f;
            ResumAll.Invoke();
        }
    }
    public void OpenSetting()
    {
        menu.SetActive(false);
        setting.SetActive(true);
    }
    public void CloseSetting()
    {

         setting.SetActive(false);
         menu.SetActive(true);


    }
}
