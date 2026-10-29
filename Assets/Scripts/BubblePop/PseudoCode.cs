using UnityEngine;

public class PseudoCode : MonoBehaviour
{
//
// Note: this was just quickly made to give an example
// It is not fully functional with the systems currently
//in the game.

    // Objects shown when the minigame is won.
    [SerializeField] private GameObject foodToShow;
    [SerializeField] private GameObject textToShow;

    private bool minigameWasWon;

    // Runs when this script first loads and hides the food and win text.
    private void Awake()
    {
        // These objects should be disabled in the table until the
        // minigame has been completed.
        SetMinigameObjectsActive(false);
    }

    // Call this method when the player wins any minigame.
    public void MinigameWon()
    {
        minigameWasWon = true;
        CheckMinigameWin();
    }

    // Checks whether the minigame was won before showing the table objects.
    private void CheckMinigameWin()
    {
        if (!minigameWasWon)
        {
            return;
        }

        SetMinigameObjectsActive(true);
    }

    // Shows or hides the food and win text GameObjects.
    private void SetMinigameObjectsActive(bool isActive)
    {
        if (foodToShow != null)
        {
            foodToShow.SetActive(isActive);
        }

        if (textToShow != null)
        {
            textToShow.SetActive(isActive);
        }
    }
}
