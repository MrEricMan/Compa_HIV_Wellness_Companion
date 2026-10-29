using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBoxUI : MonoBehaviour
{
    [Header("Dialogue box UI")]

    // Drag the Panel child here. The script shows and hides this panel.
    [SerializeField] private GameObject panel;

    // Drag the SpeakerName TextMeshPro object here.
    [SerializeField] private TMP_Text speakerNameText;

    // Drag the DialogueText TextMeshPro object here.
    [SerializeField] private TMP_Text dialogueText;

    // Drag the NextButton here.
    [SerializeField] private Button nextButton;

    private string[] currentLines;
    private int currentLineIndex;

    private void Awake()
    {
        // Connect the button so each press advances the dialogue.
        nextButton.onClick.AddListener(ShowNextLine);

        // Hide the panel until another script starts dialogue.
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Remove the listener when this object is destroyed.
        if (nextButton != null)
            nextButton.onClick.RemoveListener(ShowNextLine);
    }

    // Call this method to display dialogue from an NPC, story event, or other script.
    public void ShowDialogue(string speakerName, string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning($"No dialogue lines were provided for {speakerName}.");
            return;
        }

        speakerNameText.text = speakerName;
        currentLines = lines;
        currentLineIndex = 0;

        panel.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowNextLine()
    {
        if (currentLines == null || currentLines.Length == 0)
            return;

        currentLineIndex++;

        // Pressing Next after the final line closes the dialogue box.
        if (currentLineIndex >= currentLines.Length)
        {
            panel.SetActive(false);
            currentLines = null;
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        dialogueText.text = currentLines[currentLineIndex];
    }
}