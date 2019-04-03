using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;

    public void MenuClicked()
    {
        if (TimeControl.gameState == TimeControl.GameState.fight)
        {
            TimeControl.paused = !TimeControl.paused;
            if (TimeControl.paused)
            {
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
    }

    public void ResumeClicked()
    {
        TimeControl.paused = false;
        menu.SetActive(false);
    }

    public void MainMenuClicked()
    {
        FindObjectOfType<CharacterChoice>().GetMainMenuClicked = true;
    }
}
