using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorUIScript : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
