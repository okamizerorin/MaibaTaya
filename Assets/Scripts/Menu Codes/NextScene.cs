using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NextScene : MonoBehaviour
{
    // before loading the next scene
    public float waitTime = 5f;
    public string nextSceneName;

    void Start()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadSceneAsync(nextSceneName);
    }
}
