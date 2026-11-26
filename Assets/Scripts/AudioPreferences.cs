using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Saves and applies audio settings. Creates itself before the first scene loads
/// and survives scene changes, like GameProgressManager.
///
/// Master volume always works (AudioListener.volume).
/// Music and SFX volume work two ways:
///   1) Optional: put an AudioMixer at Assets/Resources/GameAudioMixer.mixer with exposed
///      parameters named "MusicVolume" and "SfxVolume" and route your sources through it.
///   2) Or listen to VolumeChanged / read MusicVolume and SfxVolume and apply them yourself.
/// </summary>
public class AudioPreferences : MonoBehaviour
{
    public static AudioPreferences Instance { get; private set; }

    public event Action VolumeChanged;

    private const string MasterKey = "audio.master";
    private const string MusicKey = "audio.music";
    private const string SfxKey = "audio.sfx";
    private const string MuteKey = "audio.muted";

    private const string MixerResourceName = "GameAudioMixer";
    private const string MusicParam = "MusicVolume";
    private const string SfxParam = "SfxVolume";

    private const float DefaultVolume = 1f;

    private AudioMixer _mixer;

    public float MasterVolume { get; private set; } = DefaultVolume;
    public float MusicVolume { get; private set; } = DefaultVolume;
    public float SfxVolume { get; private set; } = DefaultVolume;
    public bool Muted { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSessionReference()
    {
        Instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSession()
    {
        if (Instance != null) return;
        new GameObject("AudioPreferences").AddComponent<AudioPreferences>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _mixer = Resources.Load<AudioMixer>(MixerResourceName);
        Load();
        Apply();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Saves when the app goes to the background (important on mobile) or quits.
    private void OnApplicationPause(bool paused) { if (paused) Save(); }
    private void OnApplicationQuit() { Save(); }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        Apply();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        Apply();
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = Mathf.Clamp01(value);
        Apply();
    }

    public void SetMuted(bool muted)
    {
        Muted = muted;
        Apply();
    }

    /// Writes the current settings to disk. Call when a settings screen closes.
    public void Save()
    {
        PlayerPrefs.SetFloat(MasterKey, MasterVolume);
        PlayerPrefs.SetFloat(MusicKey, MusicVolume);
        PlayerPrefs.SetFloat(SfxKey, SfxVolume);
        PlayerPrefs.SetInt(MuteKey, Muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        MasterVolume = MusicVolume = SfxVolume = DefaultVolume;
        Muted = false;
        Apply();
        Save();
    }

    private void Load()
    {
        MasterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterKey, DefaultVolume));
        MusicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicKey, DefaultVolume));
        SfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxKey, DefaultVolume));
        Muted = PlayerPrefs.GetInt(MuteKey, 0) == 1;
    }

    private void Apply()
    {
        AudioListener.volume = Muted ? 0f : MasterVolume;

        if (_mixer != null)
        {
            _mixer.SetFloat(MusicParam, ToDecibels(MusicVolume));
            _mixer.SetFloat(SfxParam, ToDecibels(SfxVolume));
        }

        VolumeChanged?.Invoke();
    }

    // Sliders are linear (0..1) but volume is logarithmic, so convert to decibels.
    private static float ToDecibels(float linear)
    {
        return linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    }
}
