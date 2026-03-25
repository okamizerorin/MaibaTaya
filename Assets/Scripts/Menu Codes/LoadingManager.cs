using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public string sceneToLoad = "Main Menu";

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!op.isDone)
        {
            float targetProgress = Mathf.Clamp01(op.progress / 0.9f);

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime);
            progressBar.value = displayedProgress;

            if (op.progress >= 0.9f && displayedProgress >= 1f)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
