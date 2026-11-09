using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public const string BubblePopTrainId = "BubblePopTrain";
    public static GameProgressManager Instance { get; private set; }
    public int CurrentDay { get; private set; } = 1;
    public event Action<int> DayChanged;

    private readonly HashSet<string> _wonMinigames = new HashSet<string>();
    private readonly HashSet<string> _pendingFollowUps = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSessionReference()
    {
        Instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSession()
    {
        if (Instance != null) return;
        var managerObject = new GameObject("GameProgressManager");
        managerObject.AddComponent<GameProgressManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool HasWonMinigame(string minigameId)
    {
        return _wonMinigames.Contains(minigameId);
    }

    public bool HasPendingFollowUp(string minigameId)
    {
        return _pendingFollowUps.Contains(minigameId);
    }

    public bool RecordWin(string minigameId)
    {
        if (string.IsNullOrEmpty(minigameId) || !_wonMinigames.Add(minigameId)) return false;
        _pendingFollowUps.Add(minigameId);
        return true;
    }

    public bool FinishFollowUp(string minigameId)
    {
        if (!_pendingFollowUps.Remove(minigameId)) return false;
        CurrentDay++;
        DayChanged?.Invoke(CurrentDay);
        return true;
    }
}
