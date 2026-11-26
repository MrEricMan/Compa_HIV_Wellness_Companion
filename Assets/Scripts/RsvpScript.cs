using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RsvpScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textObjects;
    [SerializeField] private Image[] foodImages;

    [Tooltip("One ID per entry in the arrays above, same order. Must match GameProgressManager IDs.")]
    [SerializeField] private string[] minigameIds;

    private readonly Color32 lockedColor = new Color32(0x7a, 0x7a, 0x7a, 235);
    private readonly Color32 unlockedColor = new Color32(0xff, 0xff, 0xff, 255);

    private void Awake()
    {
        for (int i = 0; i < textObjects.Length; i++)
            SetTextAlpha(textObjects[i], 0f);

        for (int i = 0; i < foodImages.Length; i++)
            foodImages[i].color = lockedColor;
    }

    // OnEnable runs after Awake, and again whenever this object is re-activated,
    // so it works whether the RSVP UI loads with the scene or is toggled on later.
    private void OnEnable()
    {
        RefreshFromProgress();
    }

    /// Reveals every item whose minigame has been won.
    public void RefreshFromProgress()
    {
        if (GameProgressManager.Instance == null) return;

        for (int i = 0; i < minigameIds.Length; i++)
        {
            if (GameProgressManager.Instance.HasWonMinigame(minigameIds[i]))
                WinMinigame(i);
        }
    }

    /// Reveals one item by index.
    public void WinMinigame(int index)
    {
        if (index < 0 || index >= textObjects.Length || index >= foodImages.Length)
        {
            Debug.LogWarning($"RsvpScript: invalid minigame index {index}");
            return;
        }

        SetTextAlpha(textObjects[index], 1f);

        if (foodImages[index] != null)
            foodImages[index].color = unlockedColor;
    }

    private void SetTextAlpha(Component target, float alpha)
    {
        if (target is TMP_Text tmp)
        {
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;
            tmp.alpha = alpha;
        }
    }
}