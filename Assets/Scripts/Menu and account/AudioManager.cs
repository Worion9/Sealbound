using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private List<AudioClip> musicQueue = new();
    private int currentTrackIndex = 0;
    
    [SerializeField] public float volumeMultiplier;

    public enum MusicVolumeToUse
    {
        MainManu,
        Game,
        CardCollection
    }

    public MusicVolumeToUse musicVolumeToUse;

    [SerializeField] private AudioMixer myMixer;

    [Header("Audio sources")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio clips")]
    public AudioClip music1;
    public AudioClip music2;
    public AudioClip music3;
    public AudioClip music4;
    public AudioClip music5;

    public enum SFX
    {
        ButtonClick,
        PanelActivated,
        PanelDeactivated,
        CardHover,
        CardMulligan,
        CardPickUp,
        CardPlay,
        CardDraw,
        CardAddToDeck,
        CardRemoveFromDeck,
        TurnChange,
        GameWon,
        GameLost
    }

    private readonly Dictionary<SFX, AudioClip> sfxClips = new();

    [Header("SFX clips")]
    public AudioClip buttonClick;
    public AudioClip panelActivated;
    public AudioClip panelDeactivated;
    public AudioClip cardHover;
    public AudioClip cardMulligan;
    public AudioClip cardPickUp;
    public AudioClip cardPlay;
    public AudioClip cardDraw;
    public AudioClip cardAddToDeck;
    public AudioClip cardRemoveFromDeck;
    public AudioClip turnChange;
    public AudioClip gameWon;
    public AudioClip gameLost;

    private void Start()
    {
        float musicVolume = 0.3f;
        if (musicVolumeToUse == MusicVolumeToUse.MainManu) musicVolume = PlayerPrefs.GetFloat("musicInMainMenuVolume", 0.3f);
        else if (musicVolumeToUse == MusicVolumeToUse.Game) musicVolume = PlayerPrefs.GetFloat("musicInGameVolume", 0.3f);
        else if (musicVolumeToUse == MusicVolumeToUse.CardCollection) musicVolume = PlayerPrefs.GetFloat("musicInCardCollectionVolume", 0.3f);
        myMixer.SetFloat("music", Mathf.Log10(musicVolume * volumeMultiplier) * 20f);

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.3f);
        myMixer.SetFloat("sfx", Mathf.Log10(sfxVolume * volumeMultiplier) * 20f);

        InitializeMusicQueue();
        PlayNextTrack();

        InitializeSFXDictionary();
    }

    private void InitializeMusicQueue()
    {
        musicQueue = new List<AudioClip> { music1, music2, music3, music4, music5 }.OrderBy(x => Random.value).ToList();
    }

    private void InitializeSFXDictionary()
    {
        sfxClips[SFX.ButtonClick] = buttonClick;
        sfxClips[SFX.PanelActivated] = panelActivated;
        sfxClips[SFX.PanelDeactivated] = panelDeactivated;
        sfxClips[SFX.CardHover] = cardHover;
        sfxClips[SFX.CardMulligan] = cardMulligan;
        sfxClips[SFX.CardPickUp] = cardPickUp;
        sfxClips[SFX.CardPlay] = cardPlay;
        sfxClips[SFX.CardDraw] = cardDraw;
        sfxClips[SFX.CardAddToDeck] = cardAddToDeck;
        sfxClips[SFX.CardRemoveFromDeck] = cardRemoveFromDeck;
        sfxClips[SFX.TurnChange] = turnChange;
        sfxClips[SFX.GameWon] = gameWon;
        sfxClips[SFX.GameLost] = gameLost;
    }

    private void PlayNextTrack()
    {
        if (musicQueue.Count == 0) return;

        MusicSource.clip = musicQueue[currentTrackIndex];
        MusicSource.Play();

        Invoke(nameof(OnTrackFinished), MusicSource.clip.length);
    }

    private void OnTrackFinished()
    {
        currentTrackIndex++;

        if (currentTrackIndex >= musicQueue.Count) currentTrackIndex = 0;

        PlayNextTrack();
    }

    public void PlayButtonClick()
    {
        PlaySFX(SFX.ButtonClick);
    }

    public void PlaySFX(SFX sfxType)
    {
        if (sfxClips.TryGetValue(sfxType, out var clip))
        {
            SFXSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX clip for {sfxType} not found!");
        }
    }
}