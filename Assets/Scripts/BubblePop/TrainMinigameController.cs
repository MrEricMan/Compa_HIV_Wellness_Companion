using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainMinigameController : MonoBehaviour
{
    [Serializable]
    public class RoundSettings
    {
        [Min(1)] public int popQuota = 24;
        [Min(0.01f)] public float trainSpeed = 10f;
        [Min(1)] public int bubbleLimit = 24;
        [Min(0.05f)] public float spawnInterval = 0.18f;
    }

    [Header("Track and Rounds")]
    [Tooltip("Distance to the hazard. There is no separate countdown.")]
    [SerializeField, Min(1f)] private float _trackDistance = 600f;
    [SerializeField] private RoundSettings[] _rounds =
    {
        new RoundSettings { popQuota = 30, trainSpeed = 10f, bubbleLimit = 28, spawnInterval = 0.15f },
        new RoundSettings { popQuota = 24, trainSpeed = 6f, bubbleLimit = 22, spawnInterval = 0.25f },
        new RoundSettings { popQuota = 18, trainSpeed = 3f, bubbleLimit = 16, spawnInterval = 0.4f }
    };
    [Tooltip("Seconds without a successful pop before falling back one round.")]
    [SerializeField, Min(0.1f)] private float _inactivitySeconds = 6f;

    [Header("Bubble Grid")]
    [SerializeField] private Transform _gridOrigin;
    [SerializeField] private Vector2Int _gridSize = new Vector2Int(8, 4);
    [SerializeField, Min(0.1f)] private float _cellSpacing = 0.78f;
    [SerializeField] private Sprite[] _bubbleSprites;
    [SerializeField] private Material _spriteMaterial;
    [Tooltip("Enabled: hits pop the connected up/down/left/right group. Disabled: hits pop only one bubble.")]
    [SerializeField] private bool _popAdjacentBubbles = true;

    [Header("Shooter")]
    [SerializeField] private Camera _aimCamera;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private LineRenderer _aimLine;
    [SerializeField] private Sprite _projectileSprite;
    [SerializeField, Min(0.1f)] private float _shotsPerSecond = 3f;
    [SerializeField, Min(1f)] private float _projectileSpeed = 18f;
    [SerializeField, Min(0.1f)] private float _projectileLifetime = 3f;

    [Header("Presentation")]
    [SerializeField] private UnityEngine.UI.Slider _distanceBar;
    [SerializeField] private TMPro.TMP_Text _speedLabel;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMPro.TMP_Text _resultText;
    [SerializeField] private UnityEngine.UI.Button _retryButton;
    [SerializeField] private UnityEngine.UI.Button _returnButton;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Transform[] _passingObjects;
    [SerializeField] private float _sceneryLeft = -11f;
    [SerializeField] private float _sceneryRight = 11f;
    [SerializeField] private float _scenerySpeedScale = 0.5f;
    [SerializeField] private string _returnScene = "NPCInteractionScene";

    [Header("Audio")]
    [SerializeField] private AudioSource _effectsSource;
    [SerializeField] private AudioClip _popSound;
    [Tooltip("Assign your round-completion sound here.")]
    [SerializeField] private AudioClip _roundCompleteSound;

    private SpriteRenderer[] _cells;
    private readonly List<SpriteRenderer> _shots = new List<SpriteRenderer>();
    private readonly List<Vector3> _shotVelocities = new List<Vector3>();
    private readonly List<float> _shotAges = new List<float>();
    private bool _shootRequested;
    private bool _paused;
    private float _spawnElapsed;
    private float _inactivityElapsed;
    private float _fireCooldown;
    private float _distanceTravelled;
    private int _roundIndex;
    private int _roundPops;
    private Vector3[] _sceneryPositions;

    public int CurrentRound => _roundIndex;
    public int RoundPops => _roundPops;
    public float DistanceTravelled => _distanceTravelled;
    public bool HasEnded { get; private set; }
    public bool HasWon { get; private set; }
    public int ActiveBubbleCount
    {
        get { int count = 0; if (_cells != null) foreach (var cell in _cells) if (cell != null) count++; return count; }
    }

    private void Awake()
    {
        _cells = new SpriteRenderer[_gridSize.x * _gridSize.y];
        _sceneryPositions = new Vector3[_passingObjects.Length];
        for (int i = 0; i < _passingObjects.Length; i++) _sceneryPositions[i] = _passingObjects[i].position;
        _retryButton.onClick.AddListener(RestartGame);
        _returnButton.onClick.AddListener(ReturnToScene);
    }

    private void Start() { RestartGame(); }
    private void OnDestroy()
    {
        if (_retryButton != null) _retryButton.onClick.RemoveListener(RestartGame);
        if (_returnButton != null) _returnButton.onClick.RemoveListener(ReturnToScene);
    }

    public void RestartGame()
    {
        ClearShots();
        for (int i = 0; i < _cells.Length; i++) RemoveCell(i);
        _roundIndex = 0; _roundPops = 0; _distanceTravelled = 0f;
        _spawnElapsed = 0f; _inactivityElapsed = 0f; _fireCooldown = 0f;
        HasEnded = false; HasWon = false; _paused = false; _shootRequested = false;
        _resultPanel.SetActive(false);
        if (_pausePanel != null) _pausePanel.SetActive(false);
        for (int i = 0; i < _passingObjects.Length; i++) _passingObjects[i].position = _sceneryPositions[i];
        RefreshHud();
    }

    public void TogglePause()
    {
        if (HasEnded) return;
        _paused = !_paused;
        if (_pausePanel != null) _pausePanel.SetActive(_paused);
        _shootRequested = false;
    }

    private void Update()
    {
        if (HasEnded || _paused || Time.timeScale == 0f) { _shootRequested = false; return; }
        float dt = Time.deltaTime;
        _fireCooldown -= dt;
        Vector2 pointer = UnityEngine.Input.mousePosition;
        Vector3 target = _aimCamera.ScreenToWorldPoint(new Vector3(pointer.x, pointer.y, -_aimCamera.transform.position.z));
        target.z = 0f;
        Vector3 direction = (target - _firePosition.position).normalized;
        if (_aimLine != null && _aimLine.enabled)
        {
            _aimLine.SetPosition(0, _firePosition.position);
            _aimLine.SetPosition(1, direction.x < 0f ? target : _firePosition.position);
        }
        if (UnityEngine.Input.GetMouseButtonDown(0) && direction.x < 0f && _fireCooldown <= 0f &&
            (UnityEngine.EventSystems.EventSystem.current == null || !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()))
        {
            var shot = MakeSprite("Bubble Shot", _projectileSprite, _firePosition.position, 0.32f, 30, transform);
            _shots.Add(shot); _shotVelocities.Add(direction * _projectileSpeed); _shotAges.Add(0f);
            _fireCooldown = 1f / _shotsPerSecond;
        }
        _shootRequested = false;
        MoveShots(dt);
        if (HasEnded) return;
        _inactivityElapsed += dt;
        if (_inactivityElapsed >= _inactivitySeconds)
        {
            if (_roundIndex > 0) { _roundIndex--; _roundPops = 0; _spawnElapsed = 0f; }
            _inactivityElapsed = 0f;
        }
        RoundSettings round = _rounds[_roundIndex];
        _spawnElapsed += dt;
        while (_spawnElapsed >= round.spawnInterval)
        {
            _spawnElapsed -= round.spawnInterval;
            if (ActiveBubbleCount < Mathf.Min(round.bubbleLimit, _cells.Length)) SpawnRandomBubble();
        }
        _distanceTravelled = Mathf.Min(_trackDistance, _distanceTravelled + round.trainSpeed * dt);
        foreach (var passing in _passingObjects)
        {
            passing.position += Vector3.left * (round.trainSpeed * _scenerySpeedScale * dt);
            if (passing.position.x < _sceneryLeft) passing.position += Vector3.right * (_sceneryRight - _sceneryLeft);
        }
        RefreshHud();
        if (_distanceTravelled >= _trackDistance) EndGame(false);
    }

    private void SpawnRandomBubble()
    {
        int emptyCount = _cells.Length - ActiveBubbleCount;
        if (emptyCount <= 0) return;
        int choice = UnityEngine.Random.Range(0, emptyCount);
        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i] != null) continue;
            if (choice-- == 0)
            {
                Vector3 position = _gridOrigin.position + new Vector3(i % _gridSize.x, i / _gridSize.x, 0f) * _cellSpacing;
                _cells[i] = MakeSprite("Virus Bubble", _bubbleSprites[UnityEngine.Random.Range(0, _bubbleSprites.Length)], position, _cellSpacing, 20, _gridOrigin);
                return;
            }
        }
    }

    private SpriteRenderer MakeSprite(string objectName, Sprite sprite, Vector3 position, float size, int order, Transform parent)
    {
        var go = new GameObject(objectName);
        go.transform.SetParent(parent, false); go.transform.position = position;
        var renderer = go.AddComponent<SpriteRenderer>(); renderer.sprite = sprite;
        renderer.sharedMaterial = _spriteMaterial; renderer.sortingOrder = order;
        float extent = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
        go.transform.localScale = Vector3.one * (size / extent);
        return renderer;
    }

    private void MoveShots(float dt)
    {
        for (int i = _shots.Count - 1; i >= 0; i--)
        {
            Vector3 start = _shots[i].transform.position;
            Vector3 end = start + _shotVelocities[i] * dt;
            Vector3 segment = end - start;
            int hit = -1; float nearest = float.MaxValue;
            for (int cell = 0; cell < _cells.Length; cell++)
            {
                if (_cells[cell] == null) continue;
                Vector3 delta = _cells[cell].transform.position - start;
                float t = Mathf.Clamp01(Vector3.Dot(delta, segment) / Mathf.Max(segment.sqrMagnitude, 0.000001f));
                if ((start + segment * t - _cells[cell].transform.position).sqrMagnitude <= Mathf.Pow(_cellSpacing * 0.48f, 2f) && t < nearest)
                { hit = cell; nearest = t; }
            }
            _shots[i].transform.position = end; _shotAges[i] += dt;
            if (hit >= 0) { PopCluster(hit); RemoveShot(i); if (HasEnded) { ClearShots(); return; } }
            else if (_shotAges[i] >= _projectileLifetime) RemoveShot(i);
        }
    }

    private void PopCluster(int index)
    {
        var pending = new Queue<int>(); var visited = new HashSet<int>();
        pending.Enqueue(index); visited.Add(index);
        while (_popAdjacentBubbles && pending.Count > 0)
        {
            int cell = pending.Dequeue(); int x = cell % _gridSize.x; int y = cell / _gridSize.x;
            int[] neighbors = { x > 0 ? cell - 1 : -1, x < _gridSize.x - 1 ? cell + 1 : -1,
                y > 0 ? cell - _gridSize.x : -1, y < _gridSize.y - 1 ? cell + _gridSize.x : -1 };
            foreach (int neighbor in neighbors)
                if (neighbor >= 0 && _cells[neighbor] != null && visited.Add(neighbor)) pending.Enqueue(neighbor);
        }
        foreach (int cell in visited) RemoveCell(cell);
        if (_popSound != null) _effectsSource.PlayOneShot(_popSound);
        _inactivityElapsed = 0f;
        // One connected hit can finish only the current round.
        int previousPops = _roundPops;
        _roundPops += visited.Count;
        int quota = _rounds[_roundIndex].popQuota;
        if (_roundPops >= quota)
        {
            if (previousPops < quota && _roundCompleteSound != null)
                _effectsSource.PlayOneShot(_roundCompleteSound);
            if (_roundIndex == _rounds.Length - 1)
            {
                // Keep the earned quota while the player clears the remaining bubbles.
                // Spawning and inactivity regression still apply until the grid is empty.
                _roundPops = quota;
                if (ActiveBubbleCount == 0) EndGame(true);
            }
            else
            {
                _roundPops = 0; _spawnElapsed = 0f;
                _roundIndex++;
            }
        }
        RefreshHud();
    }

    private void RemoveCell(int index)
    {
        if (_cells[index] != null) { _cells[index].gameObject.SetActive(false); Destroy(_cells[index].gameObject); }
        _cells[index] = null;
    }
    private void RemoveShot(int index)
    {
        Destroy(_shots[index].gameObject); _shots.RemoveAt(index); _shotVelocities.RemoveAt(index); _shotAges.RemoveAt(index);
    }
    private void ClearShots() { for (int i = _shots.Count - 1; i >= 0; i--) RemoveShot(i); }
    private void RefreshHud()
    {
        _distanceBar.value = _distanceTravelled / _trackDistance;
        _speedLabel.SetText("Train speed: {0:0}", HasEnded ? 0f : _rounds[_roundIndex].trainSpeed);
    }
    private void EndGame(bool won)
    {
        HasEnded = true; HasWon = won; _shootRequested = false;
        if (_aimLine != null) _aimLine.enabled = false;
        _resultText.text = won ? "Train stopped!\n\nKai agreed to join the potluck." : "Track ended — Try again";
        _retryButton.gameObject.SetActive(!won); _returnButton.gameObject.SetActive(won);
        _resultPanel.SetActive(true); RefreshHud();
    }
    private void ReturnToScene() { if (HasWon) UnityEngine.SceneManagement.SceneManager.LoadScene(_returnScene); }
    private void OnValidate()
    {
        _gridSize.x = Mathf.Max(1, _gridSize.x); _gridSize.y = Mathf.Max(1, _gridSize.y);
        _trackDistance = Mathf.Max(1f, _trackDistance); _inactivitySeconds = Mathf.Max(0.1f, _inactivitySeconds);
        if (_rounds == null || _rounds.Length == 0) _rounds = new[] { new RoundSettings(), new RoundSettings(), new RoundSettings() };
        foreach (var round in _rounds) { round.popQuota = Mathf.Max(1, round.popQuota); round.trainSpeed = Mathf.Max(0.01f, round.trainSpeed); round.spawnInterval = Mathf.Max(0.05f, round.spawnInterval); round.bubbleLimit = Mathf.Max(1, round.bubbleLimit); }
    }
}
