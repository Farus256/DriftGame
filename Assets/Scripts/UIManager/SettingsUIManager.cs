using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey  = "MusicVolume";
    private const string SFXVolumeKey    = "SFXVolume";

    private void Start()
    {
        // Загружаем значения из PlayerPrefs
        float masterVol = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float musicVol  = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVol    = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        if (masterVolumeSlider) masterVolumeSlider.value = masterVol;
        if (musicVolumeSlider)  musicVolumeSlider.value  = musicVol;
        if (sfxVolumeSlider)    sfxVolumeSlider.value    = sfxVol;
       
        ApplyMasterVolume(masterVol);
        ApplyMusicVolume(musicVol);
        ApplySFXVolume(sfxVol);
    }

    public void OnMasterVolumeChanged(float value)
    {
        ApplyMasterVolume(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    private void ApplyMasterVolume(float value)
    {
        Debug.Log($"[SettingsMenu] Master Volume set to {value}");
    }

    private void ApplyMusicVolume(float value)
    {
        Debug.Log($"[SettingsMenu] Music Volume set to {value}");
    }

    private void ApplySFXVolume(float value)
    {
        Debug.Log($"[SettingsMenu] SFX Volume set to {value}");
    }
}
