using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public GameObject panel;
    public float activationDelay = 10f;

    private void Start()
    {
        if (panel == null)
        {
            return;
        }

        panel.SetActive(false);

        if (activationDelay >= 0f)
        {
            Invoke(nameof(ActivatePanel), activationDelay);
        }
    }

    private void ActivatePanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }
}
