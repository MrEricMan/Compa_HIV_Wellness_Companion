using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIScript : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
