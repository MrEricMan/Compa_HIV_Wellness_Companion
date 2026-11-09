
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform hand;
    [SerializeField] private RectTransform dial;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private float targetVolume = 1f;
    [SerializeField] private float volumeIncreaseSpeed = 0.1f;
    [SerializeField] private float volumeDecreaseSpeed = 0.2f;

    private bool volumeUp = false;
    private bool dialHeld = false;

    [Header("Duration")]
    [SerializeField] private float totalDuration = 60f;
    private float timeRemaining;

    [Header("Random Timing")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;

    [Header("Animation")]
    [SerializeField] private float slideDuration = 2f;
    [SerializeField] private Vector2 handOffset = new Vector2(30f, -20f);

    [Header("Screen Temp")]
    [SerializeField] private Button startButton;

    private Vector2 hiddenPosition;
    private Vector2 buttonPosition;

    private bool gameRunning;
    private Coroutine promptRoutine;

    #region Start

    private void Start()
    {
        hiddenPosition = hand.anchoredPosition;
        hand.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("[HandScript] StartGame called!");
        
        if (startButton != null) {startButton.gameObject.SetActive(false);}
        if (audioSource != null) 
        {
            audioSource.Play();
            audioSource.volume = 0.5f;
        }

        timeRemaining = totalDuration;
        gameRunning = true;

        StartHandPrompt();
    }

    private void EndGame()
    {
        gameRunning = false;
        StopHandPrompt();
    }

    #endregion

    #region Updates
    public void Update()
    {
        if (!gameRunning) {return;}

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(0f, timeRemaining);

        UpdateTimerUI();
        UpdateVolume();
        UpdateVolumeUI();
        IncreaseVolume();

        if (timeRemaining <= 0f) {EndGame();}
    }

    private void UpdateVolumeUI()
    {
        if (volumeText != null && audioSource != null)
        {
            volumeText.text = Mathf.RoundToInt(audioSource.volume * 100f).ToString();
        }
    }

    private void UpdateVolume()
    {
        if (audioSource == null) return;

        if (dialHeld) {audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, volumeDecreaseSpeed * Time.deltaTime);}
        else if (volumeUp){audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, volumeIncreaseSpeed * Time.deltaTime);}

        if (audioSource.volume >= targetVolume) {volumeUp = false;}
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = seconds.ToString();
        }
    }

    public void OnDialPress()
    {
        dialHeld = true;
    }

    public void OnDialRelease()
    {
        dialHeld = false;
    }

    private void IncreaseVolume()
    {
        if (volumeUp && audioSource != null)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, volumeIncreaseSpeed * Time.deltaTime);
            if (audioSource.volume >= targetVolume) {volumeUp = false;}
        }
    }
    

    #endregion

    #region Hand Movement

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

            Vector3 dialWorldPosition = dial.TransformPoint(dial.rect.center);
            Vector3 dialLocalPosition = hand.parent.InverseTransformPoint(dialWorldPosition);

            buttonPosition = (Vector2)dialLocalPosition;

            hand.gameObject.SetActive(true);

            yield return StartCoroutine(MoveHand(buttonPosition, hiddenPosition, slideDuration));

            volumeUp = true;
            yield return new WaitForSeconds(3f);

            volumeUp = false;
            yield return StartCoroutine(MoveHand(hiddenPosition, buttonPosition, slideDuration));

            hand.gameObject.SetActive(false);

            Debug.Log("Hidden Position: " + hiddenPosition);
            Debug.Log("Dial Local Position: " + dialLocalPosition);
            Debug.Log("Button Position: " + buttonPosition);
            Debug.Log("Hand Pivot: " + hand.pivot);
            Debug.Log("Hand Size: " + hand.rect.size);
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

        Debug.Log("Final Hand Position: " + hand.anchoredPosition);
        Debug.Log("Expected Position: " + buttonPosition);
    }
    #endregion
}

