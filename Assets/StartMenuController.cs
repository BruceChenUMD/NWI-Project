using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gallerySceneName = "Gallery";

    public void OnStartClick()
    {
        // Ensure the game is not still paused.
        Time.timeScale = 1f;

        // Reset the exhibit number, anomaly history,
        // initial fade, and first-round state.
        HallwayRoundManager.ResetGameProgress();

        // Load a fresh Gallery scene at Exhibit 0.
        SceneManager.LoadScene(
            gallerySceneName,
            LoadSceneMode.Single
        );
    }

    public void OnExitClick()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}