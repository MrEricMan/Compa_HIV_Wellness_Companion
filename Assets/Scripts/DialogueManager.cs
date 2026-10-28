using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask npcLayers = ~0;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button nextButton;

    private NPCDialogue activeNPC;
    private int lineIndex;

    private void Awake()
    {
        if (sceneCamera == null)
            sceneCamera = Camera.main;

        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(AdvanceDialogue);
    }

    private void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(AdvanceDialogue);
    }

    private void Update()
    {
        // Touchscreen tap
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            var touch = Touchscreen.current.primaryTouch;
            int pointerId = touch.touchId.ReadValue();

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(pointerId))
                return;

            TrySelectNPC(touch.position.ReadValue());
            return;
        }

        // Mouse click in the Unity Editor
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            TrySelectNPC(Mouse.current.position.ReadValue());
        }
    }

    private void TrySelectNPC(Vector2 screenPosition)
    {
        if (sceneCamera == null)
            return;

        Ray ray = sceneCamera.ScreenPointToRay(screenPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, npcLayers))
            return;

        NPCDialogue npc = hit.collider.GetComponentInParent<NPCDialogue>();

        if (npc != null)
            StartDialogue(npc);
    }

    private void StartDialogue(NPCDialogue npc)
    {
        if (npc.lines == null || npc.lines.Length == 0)
        {
            Debug.LogWarning($"{npc.npcName} has no dialogue lines.");
            return;
        }

        activeNPC = npc;
        lineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowCurrentLine();
    }

    private void AdvanceDialogue()
    {
        if (activeNPC == null)
            return;

        lineIndex++;

        if (lineIndex >= activeNPC.lines.Length)
        {
            dialoguePanel.SetActive(false);
            activeNPC = null;
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        nameText.text = activeNPC.npcName;
        dialogueText.text = activeNPC.lines[lineIndex];
    }
}