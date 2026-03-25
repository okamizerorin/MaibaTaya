using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
  
    public void StartGame()
    {
        SceneManager.LoadScene("Comic Section");
    }

    // later?
    public void OnExitOnClick()
    {
        Application.Quit();
    }

}
