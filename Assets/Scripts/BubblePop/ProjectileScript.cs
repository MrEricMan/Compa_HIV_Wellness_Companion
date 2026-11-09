using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private bool hasHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
        {
            return;
        }

        BubblePopBubbles bubble =
            collision.collider.GetComponent<BubblePopBubbles>();

        if (bubble == null)
        {
            return;
        }

        hasHit = true;
        bubble.PopConnectedMatchingBubbles();
        Destroy(gameObject);
    }
}