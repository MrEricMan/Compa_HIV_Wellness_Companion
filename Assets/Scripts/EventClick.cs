using UnityEngine;

public class EventClick : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
{
    [Header("NPC Interaction")]
    [Tooltip("Invoked by a left mouse click or touchscreen tap. Leave empty for no interaction.")]
    [SerializeField] private UnityEngine.Events.UnityEvent _onClick = new UnityEngine.Events.UnityEvent();

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!isActiveAndEnabled || eventData.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left)
            return;
        _onClick?.Invoke();
    }
}

