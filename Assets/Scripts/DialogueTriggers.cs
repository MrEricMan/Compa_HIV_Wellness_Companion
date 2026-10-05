using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTriggers : MonoBehaviour
{
    public TMP_Text textReferenced;
    public GameObject dialogueCanvas;
    private bool firstOpen = true;

    public void ExitButton()
    {
        dialogueCanvas.SetActive(false);
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene("BubblePopMinigameScene");
    }

     public void DialogueOption1()
    {
        textReferenced.text = "I am having some trouble understanding something. Think you can help me out?";
    }

     public void DialogueOption2()
    {
        textReferenced.text = "Thats good to hear";
    }

    public void PlayerClick()
    {
        
        if (firstOpen)
        {
            dialogueCanvas.SetActive(true);
            firstOpen = false;
        }
        else
        {
            textReferenced.text = "How is it going?";
            dialogueCanvas.SetActive(true);
        }
    }
}
