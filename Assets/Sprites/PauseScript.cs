using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject PauseContainer;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {

            PauseContainer.SetActive(true);
            Time.timeScale = 0f;
            // Free the mouse cursor to move around the UI
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    GetComponent<FirstPersonLook>().enabled = false;
        }
    }
    public void ResumeButton()
    {
        PauseContainer.SetActive(false);
            Time.timeScale = 1f;
            // Re-lock the mouse cursor back to the game window
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    GetComponent<FirstPersonLook>().enabled = true;
    }
    public void MainMenuButton()
    {
    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
     PauseContainer.SetActive(false);
            Time.timeScale = 1f;
    GetComponent<FirstPersonLook>().enabled = true;
    }
}
