using UnityEngine;
using UnityEngine.SceneManagement;

public class BubblePopReturnButton : MonoBehaviour
{
    public void ReturnScene()
    {
        SceneManager.LoadScene("NPCInteractionScene");
    }
}
