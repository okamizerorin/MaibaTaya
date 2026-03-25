using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            EndVideo();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        EndVideo();
    }

    void EndVideo()
    {
        SceneManager.LoadScene("Loading Screen");
    }
}
