using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
 
public class RsvpScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI[] textObjects;
    [SerializeField] private Image[] foodImages;
 

    private readonly Color32 lockedColor = new Color32(0x7a, 0x7a, 0x7a, 235);

    private readonly Color32 unlockedColor = new Color32(0xff, 0xff, 0xff, 255);
 
    // tracks win temp
    private int nextTestWin = 0;
 
    private void Awake()
    {
        for (int i = 0; i < textObjects.Length; i++)
        {
            SetTextAlpha(textObjects[i], 0f);
        }
 
        for (int i = 0; i < foodImages.Length; i++)
        {
            foodImages[i].color = lockedColor;
        }
    }
 
    // to be removed with tempwintracker after win conditions are added
    private void Update()
    {
        if (EPressedThisFrame() && nextTestWin < textObjects.Length)
        {
            WinMinigame(nextTestWin);
            nextTestWin++;
        }
    }
 
    private bool EPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }
 
    /// Call this when a minigame is won
    public void WinMinigame(int index)
    {
        if (index < 0 || index >= textObjects.Length || index >= foodImages.Length)
        {
            Debug.LogWarning($"MinigameProgress: invalid minigame index {index}");
            return;
        }
 
        SetTextAlpha(textObjects[index], 1f);
 
        if (foodImages[index] != null)
            foodImages[index].color = unlockedColor;
    }
 
    private void SetTextAlpha(Component target, float alpha)
    {
        if (target == null) return;
 
        if (target is TMP_Text tmp)
        {
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;
            tmp.alpha = alpha;
        }
    }
}