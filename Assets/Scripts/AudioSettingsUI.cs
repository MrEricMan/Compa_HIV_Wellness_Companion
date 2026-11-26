using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put this on your settings panel and assign the sliders/toggle in the inspector.
/// Any field you leave empty is simply skipped.
/// </summary>
public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle muteToggle;
    [SerializeField] private Button resetButton;

    private void OnEnable()
    {
        var prefs = AudioPreferences.Instance;
        if (prefs == null) return;

        // Show saved values first, then start listening, so loading
        // the UI doesn't trigger a pointless save.
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(prefs.MasterVolume);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(prefs.MusicVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(prefs.SfxVolume);
        if (muteToggle != null) muteToggle.SetIsOnWithoutNotify(prefs.Muted);

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(prefs.SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(prefs.SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(prefs.SetSfxVolume);
        if (muteToggle != null) muteToggle.onValueChanged.AddListener(prefs.SetMuted);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
    }

    private void OnDisable()
    {
        var prefs = AudioPreferences.Instance;
        if (prefs != null)
        {
            if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(prefs.SetMasterVolume);
            if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(prefs.SetMusicVolume);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(prefs.SetSfxVolume);
            if (muteToggle != null) muteToggle.onValueChanged.RemoveListener(prefs.SetMuted);
            prefs.Save(); // save when the settings panel closes
        }
        if (resetButton != null) resetButton.onClick.RemoveListener(OnResetClicked);
    }

    private void OnResetClicked()
    {
        var prefs = AudioPreferences.Instance;
        if (prefs == null) return;
        prefs.ResetToDefaults();
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(prefs.MasterVolume);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(prefs.MusicVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(prefs.SfxVolume);
        if (muteToggle != null) muteToggle.SetIsOnWithoutNotify(prefs.Muted);
    }
}
