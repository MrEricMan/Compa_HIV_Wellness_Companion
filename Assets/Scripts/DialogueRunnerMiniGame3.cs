using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueRunnerMiniGame3 : MonoBehaviour
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea(2, 4)] public string text;
    }

    [Serializable]
    public class QuestionOption
    {
        public string label;             // a note for you, never shown in game
        public bool isCorrect;
        public DialogueLine[] response;  // lines that play after this button is clicked
    }

    [Header("Jamii bubble")]
    [SerializeField] Image jamiiBubbleImage;
    [SerializeField] TMP_Text jamiiSpeakerText;
    [SerializeField] TMP_Text jamiiBodyText;
    [SerializeField] GameObject jamiiArrow;

    [Header("Compa bubble")]
    [SerializeField] Image compaBubbleImage;
    [SerializeField] TMP_Text compaSpeakerText;
    [SerializeField] TMP_Text compaBodyText;

    [Header("Objects")]
    [SerializeField] GameObject npcPanel;
    [SerializeField] GameObject compaButton;
    [SerializeField] GameObject compaRoomPanel;
    [SerializeField] GameObject choicePanel;
    [SerializeField] GameObject backButton;   // "Back to NCP"

    [Header("Bubble colors")]
    [SerializeField] Color darkColor  = new Color32(0x1F, 0x2E, 0x33, 255);
    [SerializeField] Color creamColor = new Color32(0xEA, 0xDE, 0xC8, 255);

    [Header("Settings")]
    [SerializeField] float charsPerSecond = 45f;
    [SerializeField] string mapScene = "APTCenterScene";

    [Header("Dialogue data")]
    [SerializeField] DialogueLine[] stage1Lines;    // Jamii's opening line
    [SerializeField] DialogueLine[] compaGreeting;  // "Hey! What's on your mind?"
    [SerializeField] QuestionOption[] options;      // the 4 questions
    [SerializeField] DialogueLine[] stage3Lines;    // back with Jamii

    Image bubbleImage;
    TMP_Text speakerText, bodyText;
    DialogueLine[] currentLines;
    Action onFinished;
    int index;
    bool typing, finished, autoFinish;
    Coroutine routine;

    void Start()
    {
        npcPanel.SetActive(true);
        compaButton.SetActive(false);
        compaRoomPanel.SetActive(false);
        choicePanel.SetActive(false);
        backButton.SetActive(false);
        jamiiArrow.SetActive(false);   // no arrow at the start

        UseJamiiBubble();
        Play(stage1Lines, () => compaButton.SetActive(true), true);
    }

    void UseJamiiBubble()
    {
        bubbleImage = jamiiBubbleImage;
        speakerText = jamiiSpeakerText;
        bodyText = jamiiBodyText;
    }

    void UseCompaBubble()
    {
        bubbleImage = compaBubbleImage;
        speakerText = compaSpeakerText;
        bodyText = compaBodyText;
    }

    // Hook to CompaButton
    public void OpenCompaRoom()
    {
        compaButton.SetActive(false);
        compaRoomPanel.SetActive(true);
        choicePanel.SetActive(false);
        backButton.SetActive(false);
        UseCompaBubble();
        Play(compaGreeting, ShowChoices);
    }

    void ShowChoices() { choicePanel.SetActive(true); }

    // Hook to each question button (0, 1, 2, 3)
    public void OnChoose(int i)
    {
        choicePanel.SetActive(false);
        if (options[i].isCorrect) Play(options[i].response, ShowBackButton);
        else Play(options[i].response, ShowChoices);
    }

    void ShowBackButton() { backButton.SetActive(true); }

    // Hook to "Back to NCP"
    public void GoBackToNpc()
    {
        backButton.SetActive(false);
        compaRoomPanel.SetActive(false);
        jamiiArrow.SetActive(true);    // arrow returns for the closing scene
        UseJamiiBubble();
        Play(stage3Lines, FinishMinigame);
    }

    void FinishMinigame()
    {
        SceneManager.LoadScene(mapScene);
    }

    public void Play(DialogueLine[] lines, Action done, bool autoFinishWhenTyped = false)
    {
        currentLines = lines;
        onFinished = done;
        autoFinish = autoFinishWhenTyped;
        index = -1;
        finished = false;
        ShowNext();
    }

    // Hook to Jamii's arrow and the Compa "Next" button
    public void OnTap()
    {
        if (finished) return;
        if (typing) { FinishTyping(); return; }
        if (index >= currentLines.Length - 1)
        {
            finished = true;
            onFinished?.Invoke();
            return;
        }
        ShowNext();
    }

    void ShowNext()
    {
        index++;
        string who = currentLines[index].speaker;
        speakerText.text = who.ToUpper();
        ApplyStyle(who);
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Type(currentLines[index].text));
    }

    // "You" = cream bubble, dark text. Everyone else = dark bubble, cream text.
    void ApplyStyle(string who)
    {
        bool isYou = who.Trim().ToLower() == "you";
        bubbleImage.color = isYou ? creamColor : darkColor;
        bodyText.color    = isYou ? darkColor  : creamColor;
        speakerText.color = isYou ? darkColor  : creamColor;
    }

    IEnumerator Type(string full)
    {
        typing = true;
        bodyText.text = full;
        bodyText.maxVisibleCharacters = 0;
        for (int i = 0; i <= full.Length; i++)
        {
            bodyText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(1f / charsPerSecond);
        }
        typing = false;

        // Opening scene: no arrow, so continue automatically when typing ends
        if (autoFinish && index >= currentLines.Length - 1 && !finished)
        {
            finished = true;
            onFinished?.Invoke();
        }
    }

    void FinishTyping()
    {
        if (routine != null) StopCoroutine(routine);
        bodyText.maxVisibleCharacters = int.MaxValue;
        typing = false;
    }
}