using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ComicsVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextScene = "Gameplay";

    public Image fadeOverlay;
    public float fadeDuration = 0.5f; 

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    public void SkipVideo()
    {
        videoPlayer.Stop();
        StartCoroutine(FadeAndLoad());
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        float timer = 0f;
        Color c = fadeOverlay.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Clamp01(timer / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }

        SceneManager.LoadScene(nextScene);
    }
}
