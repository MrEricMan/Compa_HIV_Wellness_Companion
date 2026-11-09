using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialDialogue : MonoBehaviour
{
    [System.Serializable]
    public class Line
    {
        public string speaker = "Valentine";
        [TextArea(2, 4)] public string text;
    }

    [SerializeField] Line[] lines;
    [SerializeField] TMP_Text speakerText;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] string nextScene = "APTCenterScene";
    [SerializeField] float charsPerSecond = 45f;

    int index = -1;
    bool typing;
    Coroutine routine;

    // Runs every time the panel is turned on
    void OnEnable()
    {
        index = -1;
        ShowNext();
    }

    // Called by the arrow button
    public void OnTap()
    {
        if (typing) { FinishTyping(); return; }               // 1st tap: finish the line
        if (index >= lines.Length - 1) { StartGame(); return; } // last line: go to the map
        ShowNext();                                            // otherwise: next line
    }

    void ShowNext()
    {
        index++;
        speakerText.text = lines[index].speaker.ToUpper();
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Type(lines[index].text));
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

    // Called at the end of the dialogue AND by the Skip button
    public void StartGame()
    {
        PlayerPrefs.SetInt("TutorialDone", 1);
        SceneManager.LoadScene(nextScene);
    }
}