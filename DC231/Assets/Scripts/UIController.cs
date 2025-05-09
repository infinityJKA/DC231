using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    private void Start()
    {
        // Sync sliders with current volume levels
        _musicSlider.value = AudioManager.Instance.musicSource.volume;
        _musicSlider.value = AudioManager.Instance.sfxSource.volume;
        _sfxSlider.value = AudioManager.Instance.sfxSource.volume;

        AudioManager.Instance.PlayMusic("Theme");
    }

    public void PlayTestSound(){
        AudioManager.Instance.PlaySFX("FootStep");
    }

    public void StopMusic(){
        AudioManager.Instance.StopMusic();
    }


    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }
}
