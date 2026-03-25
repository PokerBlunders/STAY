using UnityEngine;
using UnityEngine.SceneManagement;

public class FailCounterManager : MonoBehaviour
{
    public static FailCounterManager Instance { get; private set; }

    [Header("Animator Controllers")]
    public RuntimeAnimatorController defaultController;
    public RuntimeAnimatorController midController;
    public RuntimeAnimatorController highController;

    [Header("End Scene")]
    public string endSceneName = "EndScene";

    private int failCount = 0;
    private Animator currentAnimator;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterAnimator(Animator animator)
    {
        currentAnimator = animator;
        ApplyCurrentController();
    }

    public void AddFailure()
    {
        failCount++;
        Debug.Log($"Failure count: {failCount}");

        if (failCount > 9)
        {
            LoadEndScene();
        }
    }

    public void ResetCounter()
    {
        failCount = 0;
        ApplyCurrentController();
    }

    private void ApplyCurrentController()
    {
        if (currentAnimator == null) return;

        RuntimeAnimatorController targetController = defaultController;

        if (failCount >= 6)
            targetController = highController;
        else if (failCount >= 3)
            targetController = midController;
        else
            targetController = defaultController;

        if (targetController != null)
            currentAnimator.runtimeAnimatorController = targetController;
    }

    private void LoadEndScene()
    {
        if (FadeController.Instance != null)
            FadeController.Instance.FadeToScene(endSceneName);
        else
            SceneManager.LoadScene(endSceneName);
    }
}