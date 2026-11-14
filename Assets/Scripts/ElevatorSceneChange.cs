using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorSceneChange : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
