using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTriggers : MonoBehaviour
{
    public TMP_Text textReferenced;
    public GameObject dialogueCanvas;
    private bool firstOpen = true;
    private int clicks; // current system for selecting which dialogue plays

    public void ExitButton()
    {
        dialogueCanvas.SetActive(false);
    }

    public void ContinueButton()
    {
        clicks += 1;
        if(clicks == 1)
        {
            textReferenced.text = "I am having some trouble understanding the train of life analogy. Think you can help me out?";
        }
        if (clicks > 1)
        {
            SceneManager.LoadScene("BubblePopMinigameScene");
        }
        
    }

    //  public void DialogueOption1()
    // {
    //     textReferenced.text = "I am having some trouble understanding something. Think you can help me out?";
    // }

    //  public void DialogueOption2()
    // {
    //     textReferenced.text = "Thats good to hear";
    // }

    public void PlayerClick()
    {
        if(!dialogueCanvas.activeSelf)
        {
                if (firstOpen)
            {
                Debug.Log("Player Click");
                dialogueCanvas.SetActive(true);
                firstOpen = false;
            }
            else
            {
                textReferenced.text = "How is it going?";
                dialogueCanvas.SetActive(true);
            }
        }
        else
        {
            dialogueCanvas.SetActive(false);
        }
        
    }
}
