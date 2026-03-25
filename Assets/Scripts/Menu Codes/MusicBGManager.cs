using UnityEngine;

public class MusicBGManager : MonoBehaviour
{
    public static MusicBGManager Instance { get; private set; }

    [Header("Music")]
    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip coinSound;
    public AudioClip jumpSound;
    public AudioClip slideSound;
    public AudioClip powerupSound;
    public AudioClip playgameSound;
    public AudioClip buttonClickSound;
    public AudioClip exitClickSound;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMusic(AudioClip newMusic, bool loop = true)
    {
        if (audioSource == null || newMusic == null)
            return;

        if (audioSource.clip == newMusic && audioSource.isPlaying)
            return;

        audioSource.Stop();
        audioSource.clip = newMusic;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    // SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    // shortcuts
    public void PlayCoin() => PlaySFX(coinSound);
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlaySlide() => PlaySFX(slideSound);
    public void PlayPowerup() => PlaySFX(powerupSound);
    public void PlayGameClick() => PlaySFX(playgameSound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayExitClick() => PlaySFX(exitClickSound);
}