
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform hand;
    [SerializeField] private RectTransform dial;

    [Header("Duration")]
    [SerializeField] private float totalDuration = 60f;
    private float timeRemaining;

    [Header("Random Timing")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;

    [Header("Animation")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float pressDuration = 3f;
    [SerializeField] private Vector2 handOffset = new Vector2(30f, -20f);

    [Header("Screen Temp")]
    //[SerializeField] private GameObject startScreen;
    [SerializeField] private Button startButton;

    private Vector2 hiddenPosition;
    private Vector2 buttonPosition;

    private bool gameRunning;
    private Coroutine promptRoutine;

    private void Start()
    {
        hiddenPosition = hand.anchoredPosition;
        hand.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("[HandScript] StartGame called!");
        
        if (startButton != null) {startButton.gameObject.SetActive(false);}

        timeRemaining = totalDuration;
        gameRunning = true;

        StartHandPrompt();
    }

    public void Update()
    {
        if (!gameRunning) {return;}

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(0f, timeRemaining);

        UpdateTimerUI();

        if (timeRemaining <= 0f) {EndGame();}
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = seconds.ToString();
        }
    }

    private void EndGame()
    {
        gameRunning = false;
        StopHandPrompt();
    }

    public void StartHandPrompt()
    {
        if (promptRoutine != null) {StopCoroutine(promptRoutine);}
        promptRoutine = StartCoroutine(HandPromptRoutine());
    }

    public void StopHandPrompt()
    {
        if (promptRoutine != null) {StopCoroutine(promptRoutine);}

        promptRoutine = null;
        hand.gameObject.SetActive(false);
        hand.anchoredPosition = hiddenPosition;
    }

    private IEnumerator HandPromptRoutine()
    {
        while (gameRunning)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);

            yield return new WaitForSeconds(waitTime);
            
            if (!gameRunning) {break;}

            Vector3 dialWorldPosition = dial.position;
            Vector3 dialLocalPosition = hand.parent.InverseTransformPoint(dialWorldPosition);

            buttonPosition = (Vector2)dialLocalPosition + handOffset;
            hand.gameObject.SetActive(true);

            yield return StartCoroutine(MoveHand(hiddenPosition, buttonPosition, slideDuration));
            if (!gameRunning) {break;}

            yield return new WaitForSeconds(pressDuration);
            if (!gameRunning) {break;}

            yield return StartCoroutine(MoveHand(buttonPosition, hiddenPosition, slideDuration));

            hand.gameObject.SetActive(false);
        }

        hand.gameObject.SetActive(false);
        hand.anchoredPosition = hiddenPosition;
        promptRoutine = null;
    }

    private IEnumerator MoveHand(Vector2 start, Vector2 end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float time = Mathf.Clamp01(elapsed / duration);
            time = Mathf.SmoothStep(0f, 1f, time);
            hand.anchoredPosition = Vector2.Lerp(start, end, time);

            yield return null;
        }

        hand.anchoredPosition = end;
    }
}

