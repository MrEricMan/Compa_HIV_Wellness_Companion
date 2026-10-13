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
        if (languagePanel != null) languagePanel.SetActive(false);
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");

    }

    public void OpenAbout()     => aboutPanel.SetActive(true);
    public void CloseAbout()    => aboutPanel.SetActive(false);
    public void OpenSettings()  => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);
    public void ToggleSettings() => settingsPanel.SetActive(!settingsPanel.activeSelf);

}