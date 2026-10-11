using UnityEngine;

// Put this on an empty GameObject called "MenuManager" (NOT inside a panel).
public class MainMenuFlow : MonoBehaviour
{
    [SerializeField] GameObject aboutPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject languagePanel; // leave empty for now

    void Start()
    {
        aboutPanel.SetActive(false);
        settingsPanel.SetActive(false);
        if (languagePanel != null) languagePanel.SetActive(false);
    }

    public void OpenAbout()     => aboutPanel.SetActive(true);
    public void CloseAbout()    => aboutPanel.SetActive(false);
    public void OpenSettings()  => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);
    public void ToggleSettings() => settingsPanel.SetActive(!settingsPanel.activeSelf);
    public void ToggleAbout()    => aboutPanel.SetActive(!aboutPanel.activeSelf);

    // Called when the logo is clicked
    public void ShowLanguage()
    {
        if (languagePanel == null)
        {
            Debug.Log("Logo clicked! Language panel not built yet.");
            return;
        }
        languagePanel.SetActive(true);
    }
}