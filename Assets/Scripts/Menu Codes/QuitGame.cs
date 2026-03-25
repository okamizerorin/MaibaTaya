using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    public GameObject confirmationPanel;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ShowConfirmation();
        }
    }

    public void ShowConfirmation()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void ConfirmExit()
    {
        MusicBGManager.Instance?.PlayButtonClick();
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Quit Game called");
    }

    public void CancelExit()
    {
        MusicBGManager.Instance?.PlayButtonClick();
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}