
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    // A static reference to the SettingsManager.
    // This allows other scripts to access it without needing
    // a direct reference to the GameObject.
    public static SettingsManager Instance;

    // Reference to the Unity Audio Mixer.
    // [SerializeField] allows us to assign it in the Inspector
    // while keeping the variable private.
    [SerializeField] private AudioMixer audioMixer;

    // Store the current volume settings in variables.
    // These values remain intact when changing scenes.
    private float musicVolume = 1f;
    private float sfxVolume = 1f;


    // Awake() runs when this object is initialized.
    private void Awake()
    {
        // Check if another SettingsManager already exists.
        if (Instance != null)
        {
            // If one exists, destroy this duplicate.
            Destroy(gameObject);
            return;
        }

        // If this is the first instance, make it the official one.
        Instance = this;

        // Prevent this GameObject from being destroyed
        // when transitioning between scenes.
        DontDestroyOnLoad(gameObject);

        // Apply the default volume settings.
        ApplyAudioSettings();
    }


    // Called whenever the music volume slider changes.
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        ApplyVolume("MusicVolume", musicVolume);
    }


    // Called whenever the SFX volume slider changes.
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        ApplyVolume("SFXVolume", sfxVolume);
    }


    // Converts a slider value (0-1) into decibels
    // and applies it to the corresponding Audio Mixer parameter.
    private void ApplyVolume(string parameterName, float volume)
    {
        float decibels = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;

        audioMixer.SetFloat(parameterName, decibels);
    }


    // Applies all stored audio settings.
    // Useful when initializing the manager.
    private void ApplyAudioSettings()
    {
        ApplyVolume("MusicVolume", musicVolume);
        ApplyVolume("SFXVolume", sfxVolume);
    }


    // Allows other scripts to retrieve the current volume settings.
    // This is useful for updating sliders when opening a settings panel.
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}