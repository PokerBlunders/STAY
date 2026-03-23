using UnityEngine;
using UnityEngine.SceneManagement;

public class FailCounterManager : MonoBehaviour
{
    public static FailCounterManager Instance { get; private set; }

    [Header("Animator Controllers")]
    public RuntimeAnimatorController defaultController;   // Used for 0-2 failures
    public RuntimeAnimatorController midController;       // Used for 3-5 failures
    public RuntimeAnimatorController highController;      // Used for 6-9 failures

    [Header("End Scene")]
    public string endSceneName = "EndScene";              // Scene to load at 10 failures

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

    // Call this from any scene to register the player's animator
    public void RegisterAnimator(Animator animator)
    {
        currentAnimator = animator;
        ApplyCurrentController();
    }

    // Call this when a QTE fails
    public void AddFailure()
    {
        failCount++;
        Debug.Log($"Failure count: {failCount}");

        if (failCount > 9)
        {
            LoadEndScene();
        }
        // No immediate animator update – the change will take effect on next scene load
    }

    // Reset counter (useful for restarting the game)
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
        // Optional: use your FadeController if available
        if (FadeController.Instance != null)
            FadeController.Instance.FadeToScene(endSceneName);
        else
            SceneManager.LoadScene(endSceneName);
    }
}