using UnityEngine;

public class LobbyDayController : MonoBehaviour
{
    [Header("Lobby NPCs")]
    [SerializeField] private GameObject _compaNpc;
    [SerializeField] private GameObject _kaiNpc;

    [Header("Existing Dialogue Sources")]
    [Tooltip("Always-active source containing Compa's Inspector day/lines entries.")]
    [SerializeField] private DialogueTriggers _compaConversation;
    [Tooltip("Always-active source containing Kai's Inspector day/lines entries.")]
    [SerializeField] private DialogueTriggers _kaiConversation;
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMPro.TMP_Text _speakerText;

    [Header("Modal UI")]
    [Tooltip("NPC interaction is blocked while any of these pause/settings/community panels are open.")]
    [SerializeField] private GameObject[] _blockingPanels = new GameObject[0];

    private GameProgressManager _progress;
    private DialogueTriggers _activeConversation;
    private bool _configured;

    private void Awake()
    {
        _configured = _compaNpc != null && _kaiNpc != null &&
            _compaConversation != null && _kaiConversation != null &&
            _dialoguePanel != null && _speakerText != null;
        if (!_configured)
        {
            Debug.LogError("Lobby day controller requires both NPCs, conversation sources, dialogue panel, and speaker text.", this);
            return;
        }
        _dialoguePanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (!_configured) return;
        _progress = GameProgressManager.Instance;
        if (_progress == null)
        {
            Debug.LogError("Lobby day controller requires the session GameProgressManager.", this);
            return;
        }
        _progress.DayChanged += ApplyDay;
        ApplyDay(_progress.CurrentDay);
    }

    private void OnDisable()
    {
        if (_progress != null) _progress.DayChanged -= ApplyDay;
        CloseDialogue();
    }

    private void ApplyDay(int day)
    {
        CloseDialogue();
        _compaNpc.SetActive(day == 1);
        _kaiNpc.SetActive(day >= 2);
    }

    public void OpenCompaDialogue()
    {
        OpenDialogue(_compaNpc, _compaConversation, "Compa");
    }

    public void OpenKaiDialogue()
    {
        OpenDialogue(_kaiNpc, _kaiConversation, "Kai");
    }

    private void OpenDialogue(GameObject npc, DialogueTriggers source, string speaker)
    {
        if (!_configured || _progress == null || !npc.activeInHierarchy || IsBlocked()) return;
        // Repeated NPC taps close the same conversation, matching the room behavior.
        if (_activeConversation == source && _dialoguePanel.activeSelf)
        {
            CloseDialogue();
            return;
        }
        CloseDialogue();
        _activeConversation = source;
        _speakerText.text = speaker;
        source.PlayerClick();
        if (!_dialoguePanel.activeSelf) _activeConversation = null;
    }

    public void ContinueDialogue()
    {
        if (_activeConversation == null || IsBlocked()) return;
        _activeConversation.ContinueButton();
        if (!_dialoguePanel.activeSelf) _activeConversation = null;
    }

    public void CloseDialogue()
    {
        if (_activeConversation != null) _activeConversation.ExitButton();
        _activeConversation = null;
        if (_dialoguePanel != null) _dialoguePanel.SetActive(false);
    }

    private bool IsBlocked()
    {
        if (Time.timeScale == 0f) return true;
        foreach (var panel in _blockingPanels)
            if (panel != null && panel.activeInHierarchy) return true;
        return false;
    }
}
