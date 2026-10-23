using UnityEngine;
using UnityEngine.UI;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damagePerBubble = 10f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
{
    BubblePopBubbles bubble = other.GetComponentInParent<BubblePopBubbles>();

    if (bubble == null)
    {
        return;
    }

    currentHealth = Mathf.Max(0f, currentHealth - damagePerBubble);

    if (healthBar != null)
    {
        healthBar.value = currentHealth;
    }

    Destroy(bubble.gameObject);
}
}