using UnityEngine;
using UnityEngine.EventSystems;

public class SpeechToText : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public AvatarBrain avatarBrain;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (avatarBrain != null) {avatarBrain.OnSpeechButtonDown();}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (avatarBrain != null) {avatarBrain.OnSpeechButtonUp();}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (avatarBrain != null) {avatarBrain.OnSpeechButtonUp();}
    }

    public void OnDisable(PointerEventData eventData)
    {
        if (avatarBrain != null) {avatarBrain.OnSpeechButtonUp();}
    }
}
