using UnityEngine;

public class TrainTrackScroller : MonoBehaviour
{
    [Header("Repeating Track")]
    [SerializeField] private Transform[] _tiles;
    [SerializeField, Min(0.01f)] private float _tileWidth = 3.04f;

    private Vector3[] _initialPositions;
    private float _offset;

    public void ResetTrack()
    {
        if (_initialPositions == null)
        {
            _initialPositions = new Vector3[_tiles.Length];
            for (int i = 0; i < _tiles.Length; i++)
                _initialPositions[i] = _tiles[i].localPosition;
        }
        _offset = 0f;
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i].localPosition = _initialPositions[i];
    }

    public void Scroll(float distance)
    {
        if (_tiles == null || _tiles.Length == 0) return;
        if (_initialPositions == null) ResetTrack();
        // The strip repeats every tile, not at the warning signs' wrap bounds.
        // Keeping the offset small avoids accumulated drift at tile seams.
        _offset = Mathf.Repeat(_offset + distance, _tileWidth);
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i].localPosition = _initialPositions[i] + Vector3.left * _offset;
    }
}
