using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTriggers : MonoBehaviour
{
    [Serializable]
    public class DayDialogue
    {
        [Min(1)] public int day = 2;
        [TextArea] public string[] lines = { "insert placeholder dialogue" };
    }

    // Preserve the existing scene references and button callbacks.
    public TMPro.TMP_Text textReferenced;
    public GameObject dialogueCanvas;

    [Header("Shared Conversation")]
    [Tooltip("All buttons for one NPC use the same conversation source. Leave empty on the source itself.")]
    [SerializeField] private DialogueTriggers _conversationSource;
    [SerializeField] private string _minigameId = GameProgressManager.BubblePopTrainId;
    [SerializeField] private string _minigameScene = "BubblePopMinigameScene";

    [Header("Dialogue")]
    [TextArea]
    [SerializeField] private string _minigameInvitation = "I am having some trouble understanding the train of life analogy. Think you can help me out?";
    [SerializeField] private string[] _postWinDialogue = { "insert placeholder dialogue" };
    [SerializeField] private DayDialogue[] _dayDialogues = { new DayDialogue() };

    [Header("Next Day")]
    [Tooltip("Scene to visit after the final post-win dialogue line. Change this when the day-start location changes.")]
    [SerializeField] private string _nextDayScene = "APTCenterScene";

    private enum ConversationType { Introduction, PostWin, Daily }
    private ConversationType _conversationType;
    private string _introductoryDialogue;
    private string[] _activeLines;
    private int _lineIndex;
    private bool _firstOpen = true;
    private GameProgressManager _progress;

    private bool IsSource => _conversationSource == null || _conversationSource == this;
    private DialogueTriggers Source => IsSource ? this : _conversationSource;

    private void Awake()
    {
        if (!IsSource) return;
        _progress = GameProgressManager.Instance;
        if (textReferenced != null) _introductoryDialogue = textReferenced.text;
    }

    private void OnEnable()
    {
        if (!IsSource) return;
        _progress = GameProgressManager.Instance;
        if (_progress != null) _progress.DayChanged += OnDayChanged;
    }

    private void Start()
    {
        if (!IsSource) return;
        if (textReferenced == null || dialogueCanvas == null)
        {
            Debug.LogError("NPC conversation requires its dialogue text and panel references.", this);
            return;
        }
        if (_progress.HasPendingFollowUp(_minigameId)) OpenConversation();
        else dialogueCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        if (IsSource && _progress != null) _progress.DayChanged -= OnDayChanged;
    }

    public void ExitButton()
    {
        Source.CloseConversation();
    }

    public void ContinueButton()
    {
        Source.AdvanceConversation();
    }

    public void PlayerClick()
    {
        var source = Source;
        if (source.dialogueCanvas.activeSelf) source.CloseConversation();
        else source.OpenConversation();
    }

    private void OpenConversation()
    {
        _lineIndex = 0;
        if (_progress.HasPendingFollowUp(_minigameId))
        {
            _conversationType = ConversationType.PostWin;
            _activeLines = _postWinDialogue;
        }
        else if (_progress.CurrentDay == 1 && !_progress.HasWonMinigame(_minigameId))
        {
            _conversationType = ConversationType.Introduction;
            _activeLines = new[] { _firstOpen ? _introductoryDialogue : "How is it going?", _minigameInvitation };
            _firstOpen = false;
        }
        else
        {
            _conversationType = ConversationType.Daily;
            _activeLines = null;
            foreach (var dialogue in _dayDialogues)
            {
                if (dialogue != null && dialogue.day == _progress.CurrentDay)
                {
                    _activeLines = dialogue.lines;
                    break;
                }
            }
        }
        if (_activeLines == null || _activeLines.Length == 0)
            _activeLines = new[] { "insert placeholder dialogue" };
        textReferenced.text = _activeLines[0];
        dialogueCanvas.SetActive(true);
    }

    private void AdvanceConversation()
    {
        if (!dialogueCanvas.activeSelf || _activeLines == null) return;
        if (_lineIndex + 1 < _activeLines.Length)
        {
            _lineIndex++;
            textReferenced.text = _activeLines[_lineIndex];
            return;
        }
        if (_conversationType == ConversationType.Introduction)
        {
            if (!CanLoadScene(_minigameScene)) return;
            CloseConversation();
            SceneManager.LoadScene(_minigameScene);
        }
        else if (_conversationType == ConversationType.PostWin)
        {
            // Keep the follow-up pending if the destination is not available.
            if (!CanLoadScene(_nextDayScene)) return;
            CloseConversation();
            if (_progress.FinishFollowUp(_minigameId)) SceneManager.LoadScene(_nextDayScene);
        }
        else CloseConversation();
    }

    private bool CanLoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName) && Application.CanStreamedLevelBeLoaded(sceneName)) return true;
        Debug.LogError("Conversation destination is not available in the build: " + sceneName, this);
        return false;
    }

    private void CloseConversation()
    {
        dialogueCanvas.SetActive(false);
        _activeLines = null;
    }

    private void OnDayChanged(int day)
    {
        // Any NPC using this component reads the shared day on its next conversation.
        CloseConversation();
    }
}
