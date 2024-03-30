using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    #region Vari�veis Globais
    [Header("Configura��es")]

    [Header("Transi��o:")]
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float loadTime;

    private bool _gamePaused;
    #endregion

    #region Fun��es Unity
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_gamePaused)
                Pause();
            else
                Resume();
        }
    }
    #endregion

    #region Fun��es Pr�prias
    private void Pause()
    {
        Time.timeScale = 0f;
        GameMenuManager.MenuCanvas.SetActive(true);
        _gamePaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameMenuManager.MenuCanvas.SetActive(false);
        _gamePaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToOptions() => GameMenuManager.Instance.OpenMenu(InGame.Options, GameMenuManager.GameMenu);

    public void GoToControls() => GameMenuManager.Instance.OpenMenu(InGame.Controls, GameMenuManager.GameMenu);

    public void GoToMainMenu() => TransitionManager.Instance().Transition("Main Menu", transitionSettings, loadTime);
    #endregion
}
