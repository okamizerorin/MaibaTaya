using UnityEngine;
using UnityEngine.Video;

public class ComicVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource videoAudioSource;

    void Start()
    {
        MusicBGManager.Instance.StopMusic();

        videoAudioSource.volume = MusicBGManager.Instance.audioSource.volume;

        videoPlayer.Play();
    }
}