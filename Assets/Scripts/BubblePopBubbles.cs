using UnityEngine;

public class BubblePopBubbles : MonoBehaviour
{
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite4;
    [SerializeField] private Sprite sprite5;

    public float moveSpeed = 1f;
    public AudioClip popSound;

    private void Start()
    {
        Sprite[] sprites = { sprite1, sprite2, sprite3, sprite4, sprite5 };
        Sprite chosenSprite = sprites[Random.Range(0, sprites.Length)];

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = chosenSprite;
        }
    }

    private void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    private void OnMouseDown()
    {
        if (popSound != null)
        {
            AudioSource.PlayClipAtPoint(popSound, transform.position);
        }

        Destroy(gameObject);
    }
}
