using UnityEngine;
using UnityEngine.SceneManagement;

public class MapButtonsScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApartmentButton()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void ClinicButton()
    {
        SceneManager.LoadScene("InteractionPrototype");
    }
}
