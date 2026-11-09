using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Put this on the logo Image. Needs: Raycast Target ON, and a CanvasGroup (auto-added).
[RequireComponent(typeof(CanvasGroup))]
public class MenuLogo : MonoBehaviour, IPointerClickHandler
{
    [Header("Idle animation")]
    [SerializeField] float pulseAmount = 0.04f;   // 4% breathing
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float bobAmount = 8f;        // pixels up/down
    [SerializeField] bool reduceMotion = false;   // hook this to an accessibility setting later

    [Header("Press animation")]
    [SerializeField] float pressScale = 1.3f;
    [SerializeField] float pressDuration = 0.35f;

    public UnityEvent onLogoPressed;              

    RectTransform rt;
    CanvasGroup group;
    Vector2 startPos;
    bool pressed;

    void Awake()
    {
        rt = (RectTransform)transform;
        group = GetComponent<CanvasGroup>();
        startPos = rt.anchoredPosition;
    }

    void Update()
    {
        if (pressed || reduceMotion) return;
        float t = Time.unscaledTime * pulseSpeed;
        rt.localScale = Vector3.one * (1f + Mathf.Sin(t) * pulseAmount);
        rt.anchoredPosition = startPos + Vector2.up * (Mathf.Sin(t * 0.5f) * bobAmount);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (pressed) return;
        pressed = true;
        StartCoroutine(PressRoutine());
    }

    IEnumerator PressRoutine()
    {
        Vector3 from = rt.localScale;
        Vector3 to = Vector3.one * pressScale;
        for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / pressDuration)
        {
            float eased = 1f - Mathf.Pow(1f - t, 3f); // ease-out
            rt.localScale = Vector3.Lerp(from, to, eased);
            group.alpha = 1f - eased;
            yield return null;
        }
        group.alpha = 0f;
        onLogoPressed?.Invoke();
    }

    public void ResetLogo()
    {
        StopAllCoroutines();
        pressed = false;
        group.alpha = 1f;
        rt.localScale = Vector3.one;
        rt.anchoredPosition = startPos;
    }
}