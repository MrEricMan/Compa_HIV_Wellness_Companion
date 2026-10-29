using UnityEngine;
using UnityEngine.SceneManagement;

public class CompaButtonScript : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
