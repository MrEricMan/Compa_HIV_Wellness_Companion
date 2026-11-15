using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButtons : MonoBehaviour
{

    public void NPCRoomsButton()
    {
        SceneManager.LoadScene("NPCInteractionScene");
    }

    public void PlayerRoomButton()
    {
        SceneManager.LoadScene("PlayerRoom");
    }

    public void BackButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

