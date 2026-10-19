using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFlow : MonoBehaviour
{
    [SerializeField] GameObject aboutPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject languagePanel; // leave empty for now

    void Start()
    {
        aboutPanel.SetActive(false);
        settingsPanel.SetActive(false);
        languagePanel.SetActive(false);

    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("APTCenterScene");

    }

    public void OpenAbout()     => aboutPanel.SetActive(true);
    public void CloseAbout()    => aboutPanel.SetActive(false);
    public void OpenLanguage()    => languagePanel.SetActive(true);

    public void OpenSettings()  => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);
    public void ToggleSettings() => settingsPanel.SetActive(!settingsPanel.activeSelf);

}