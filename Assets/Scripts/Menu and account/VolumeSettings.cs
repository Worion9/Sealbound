using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioManager audioMenager;

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider musicInMenuSlider;
    [SerializeField] Slider musicInGameSlider;
    [SerializeField] Slider musicInCardCollectionSlider;
    [SerializeField] Slider SFXSlider;

    [SerializeField] TextMeshProUGUI musicInMenuText;
    [SerializeField] TextMeshProUGUI musicInGameText;
    [SerializeField] TextMeshProUGUI musicInCardCollectionText;
    [SerializeField] TextMeshProUGUI SFXText;

    private void Start()
    {
        LoadValue();
    }

    public void SetMusicInMainMenuVolume()
    {
        float volume = musicInMenuSlider.value;
        PlayerPrefs.SetFloat("musicInMainMenuVolume", volume);
        musicInMenuText.text = $"Music In Main Menu Volume: {Mathf.CeilToInt(volume * 100f - 0.01f)}%";

        audioMixer.SetFloat("music", Mathf.Log10(volume * audioMenager.volumeMultiplier) * 20f);
    }

    public void SetMusicInGameVolume()
    {
        float volume = musicInGameSlider.value;
        PlayerPrefs.SetFloat("musicInGameVolume", volume);
        musicInGameText.text = $"Music In Game Volume: {Mathf.CeilToInt(volume * 100f - 0.01f)}%";
    }

    public void SetMusicInCardCollectionVolume()
    {
        float volume = musicInCardCollectionSlider.value;
        PlayerPrefs.SetFloat("musicInCardCollectionVolume", volume);
        musicInCardCollectionText.text = $"Music In Card Collection Volume: {Mathf.CeilToInt(volume * 100f - 0.01f)}%";
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        SFXText.text = $"SFX Volume: {Mathf.CeilToInt(volume * 100f - 0.01f)}%";

        audioMixer.SetFloat("sfx", Mathf.Log10(volume * audioMenager.volumeMultiplier) * 20f);
    }

    private void LoadValue()
    {
        musicInMenuSlider.value = PlayerPrefs.GetFloat("musicInMainMenuVolume", 0.3f);
        musicInGameSlider.value = PlayerPrefs.GetFloat("musicInGameVolume", 0.3f);
        musicInCardCollectionSlider.value = PlayerPrefs.GetFloat("musicInCardCollectionVolume", 0.3f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.3f);

        SetMusicInMainMenuVolume();
        SetMusicInGameVolume();
        SetMusicInCardCollectionVolume();
        SetSFXVolume();
    }
}