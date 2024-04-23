using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !PlayerManager.Instance.IsDead)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SaveManager.Instance.LoadSave();

        Debug.Log("Restart game ....");
    }
    public void Setting()
    {
        Debug.Log("Setting");
    }
    public void Quit()
    {
        Debug.Log("Quitting ....");
        Application.Quit();
    }
}
