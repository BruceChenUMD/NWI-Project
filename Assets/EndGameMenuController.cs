using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenuController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject completionPanel;

    [Header("Scenes")]
    [SerializeField] private string gameplaySceneName = "Gallery";

    private bool loadingScene;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (completionPanel != null)
            completionPanel.SetActive(false);

        UnlockCursor();
    }

    private void LateUpdate()
    {
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
        if (loadingScene)
            return;

        loadingScene = true;
        Time.timeScale = 1f;

        // Reset streak, anomaly history, and first-round state
        // before loading Gallery.
        HallwayRoundManager.ResetGameProgress();

        Debug.Log(
            "Play Again: progress reset. Loading Exhibit 0."
        );

        SceneManager.LoadScene(
            gameplaySceneName,
            LoadSceneMode.Single
        );
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