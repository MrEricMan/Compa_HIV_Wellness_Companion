using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 4)] public string text;
}

[Serializable]
public class QuestionOption
{
    public string label;
    public bool isCorrect;
    public DialogueLine[] response;
}

public class DialogueRunnerMiniGame3 : MonoBehaviour
{
    [Header("Jamii bubble")]
    [SerializeField] Image jamiiBubbleImage;
    [SerializeField] TMP_Text jamiiSpeakerText;
    [SerializeField] TMP_Text jamiiBodyText;

    [Header("Compa bubble")]
    [SerializeField] Image compaBubbleImage;
    [SerializeField] TMP_Text compaSpeakerText;
    [SerializeField] TMP_Text compaBodyText;

    [Header("Objects")]
    [SerializeField] GameObject npcPanel;
    [SerializeField] GameObject compaButton;
    [SerializeField] GameObject compaRoomPanel;
    [SerializeField] GameObject choicePanel;

    [Header("Bubble colors")]
    [SerializeField] Color darkColor  = new Color32(0x1F, 0x2E, 0x33, 255);
    [SerializeField] Color creamColor = new Color32(0xEA, 0xDE, 0xC8, 255);

    [Header("Settings")]
    [SerializeField] float charsPerSecond = 45f;
    [SerializeField] string mapScene = "APTCenterScene";
    [SerializeField] string rsvpKey = "RSVP_Jamii";

    [Header("Dialogue data")]
    [SerializeField] DialogueLine[] stage1Lines;
    [SerializeField] DialogueLine[] compaGreeting;
    [SerializeField] QuestionOption[] options;
    [SerializeField] DialogueLine[] stage3Lines;

    Image bubbleImage;
    TMP_Text speakerText, bodyText;
    DialogueLine[] currentLines;
    Action onFinished;
    int index;
    bool typing, finished;
    Coroutine routine;

    void Start()
    {
        npcPanel.SetActive(true);
        compaButton.SetActive(false);
        compaRoomPanel.SetActive(false);
        choicePanel.SetActive(false);

        UseJamiiBubble();
        Play(stage1Lines, () => compaButton.SetActive(true));
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
        UseCompaBubble();
        Play(compaGreeting, ShowChoices);
    }

    // Hook to "Back to NCP"
    public void BackToNpc()
    {
        if (routine != null) StopCoroutine(routine);
        typing = false;
        finished = true;               // stops stray taps from advancing hidden lines
        choicePanel.SetActive(false);
        compaRoomPanel.SetActive(false);
        compaButton.SetActive(true);
        UseJamiiBubble();
    }

    void ShowChoices() { choicePanel.SetActive(true); }

    // Hook to each question button (0, 1, 2, 3)
    public void OnChoose(int i)
    {
        choicePanel.SetActive(false);
        if (options[i].isCorrect) Play(options[i].response, BackToJamii);
        else Play(options[i].response, ShowChoices);
    }

    void BackToJamii()
    {
        compaRoomPanel.SetActive(false);
        UseJamiiBubble();
        Play(stage3Lines, FinishMinigame);
    }

    void FinishMinigame()
    {
        PlayerPrefs.SetInt(rsvpKey, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(mapScene);
    }

    public void Play(DialogueLine[] lines, Action done)
    {
        currentLines = lines;
        onFinished = done;
        index = -1;
        finished = false;
        ShowNext();
    }

    // Hook to BOTH arrow ("Next") buttons
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
    }

    void FinishTyping()
    {
        if (routine != null) StopCoroutine(routine);
        bodyText.maxVisibleCharacters = int.MaxValue;
        typing = false;
    }
}