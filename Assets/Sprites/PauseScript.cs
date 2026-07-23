using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject PauseContainer;

    [SerializeField] private GameObject mainPauseScreen;
    [SerializeField] private GameObject aboutScreen;
    [SerializeField] private GameObject leavingScreen;

    [Header("Player")]
    [SerializeField] private FirstPersonLook playerLook;

    private bool isPaused;

    private void Start()
    {
        SetPaused(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!isPaused);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (mainPauseScreen != null)
            mainPauseScreen.SetActive(true);

        if (aboutScreen != null)
            aboutScreen.SetActive(false);

        if (leavingScreen != null)
            leavingScreen.SetActive(false);

        if (PauseContainer != null)
            PauseContainer.SetActive(paused);

        Time.timeScale = paused ? 0f : 1f;

        if (playerLook != null)
            playerLook.enabled = !paused;

        Cursor.lockState = paused
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = paused;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeButton()
    {
        SetPaused(false);
    }

    public void MainMenuButton()
    {
        SetPaused(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Main Menu");
    }
}