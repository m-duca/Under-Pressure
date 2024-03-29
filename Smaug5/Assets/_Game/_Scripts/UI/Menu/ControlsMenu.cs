using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public void GoBackToMainMenu() => MainMenuManager.Instance.OpenMenu(Default.MainMenu, MainMenuManager.MainMenu);

    public void GoBackToGameMenu() => GameMenuManager.Instance.OpenMenu(InGame.GameMenu, GameMenuManager.OptionsMenu);
}
