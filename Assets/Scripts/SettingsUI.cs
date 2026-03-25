using UnityEngine;

public class SettingsUI : MonoBehaviour
{

    public void MusicOn()
    {
        AudioManager.instance.SetMusic(true);
    }

    public void MusicOff()
    {
        AudioManager.instance.SetMusic(false);
    }

    public void SFX_Max()
    {
        AudioManager.instance.SetSFXVolume(1f);
    }

    public void SFX_Mid()
    {
        AudioManager.instance.SetSFXVolume(0.5f);
    }

    public void SFX_Mute()
    {
        AudioManager.instance.SetSFXVolume(0f);
    }
}