using UnityEngine;
using System.Collections.Generic;

public class BubblePopBubbles : MonoBehaviour
{
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite4;
    [SerializeField] private Sprite sprite5;
    [SerializeField] private Vector2 neighborSpacing = new Vector2(2f, 2f);
    [SerializeField] private float neighborSearchRadius = 0.15f;

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

    

    public void PopConnectedMatchingBubbles()
    {
        SpriteRenderer ownRenderer = GetComponent<SpriteRenderer>();
        if (ownRenderer == null || ownRenderer.sprite == null)
        {
            return;
        }

        if (popSound != null)
        {
            AudioSource.PlayClipAtPoint(popSound, transform.position);
        }

        Queue<BubblePopBubbles> bubblesToCheck = new Queue<BubblePopBubbles>();
        HashSet<BubblePopBubbles> connectedBubbles = new HashSet<BubblePopBubbles>();

        bubblesToCheck.Enqueue(this);
        connectedBubbles.Add(this);

        Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        while (bubblesToCheck.Count > 0)
        {
            BubblePopBubbles currentBubble = bubblesToCheck.Dequeue();
            Collider currentCollider = currentBubble.GetComponent<Collider>();

            if (currentCollider == null)
            {
                continue;
            }

            Vector3 center = currentCollider.bounds.center;

            foreach (Vector3 direction in directions)
            {
                Vector3 offset = new Vector3(
                    direction.x * neighborSpacing.x,
                    direction.y * neighborSpacing.y,
                    0f
                );

                Vector3 neighborPosition = center + offset;
                Collider[] colliders =
                    Physics.OverlapSphere(neighborPosition, neighborSearchRadius);

                foreach (Collider neighborCollider in colliders)
                {
                    BubblePopBubbles neighbor =
                        neighborCollider.GetComponent<BubblePopBubbles>();

                    if (neighbor == null || connectedBubbles.Contains(neighbor))
                    {
                        continue;
                    }

                    SpriteRenderer neighborRenderer =
                        neighbor.GetComponent<SpriteRenderer>();

                    if (neighborRenderer != null &&
                        neighborRenderer.sprite == ownRenderer.sprite)
                    {
                        connectedBubbles.Add(neighbor);
                        bubblesToCheck.Enqueue(neighbor);
                    }
                }
            }
        }

        foreach (BubblePopBubbles bubble in connectedBubbles)
        {
            Destroy(bubble.gameObject);
        }
    }
}
