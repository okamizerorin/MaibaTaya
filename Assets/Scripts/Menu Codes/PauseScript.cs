using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{

    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject platformNameUI;

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        gameUI.SetActive(false);
        platformNameUI.SetActive(false);
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        gameUI.SetActive(true);
        platformNameUI.SetActive(true);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
