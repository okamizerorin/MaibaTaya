using UnityEngine;

public class GameplayAudioLoader : MonoBehaviour
{
    void Start()
    {
        MusicBGManager.Instance.PlayGameplayMusic();
    }
}