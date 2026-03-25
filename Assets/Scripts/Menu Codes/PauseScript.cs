using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{

    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameUI;

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        gameUI.SetActive(false);
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        gameUI.SetActive(true);
    }

    public void HomeMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
