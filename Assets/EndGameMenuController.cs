using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenuController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject completionPanel;

    [Header("Scenes")]
    [SerializeField] private string gameplaySceneName = "Gallery";

    private void Awake()
    {
        Time.timeScale = 1f;

        if (completionPanel != null)
            completionPanel.SetActive(false);

        UnlockCursor();
    }

    private void OnEnable()
    {
        UnlockCursor();
    }

    private void LateUpdate()
    {
        // Stops the persistent first-person controller
        // from locking the cursor again.
        UnlockCursor();
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowCompletionScreen()
    {
        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;

        HallwayRoundManager.ResetGameProgress();

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void CloseGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}