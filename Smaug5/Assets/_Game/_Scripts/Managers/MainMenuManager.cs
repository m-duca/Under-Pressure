using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    #region Vari�veis Globais
    // Inst�ncia da Classe
    public static MainMenuManager Instance;

    // Refer�ncias dos Menus que ser�o manipulados
    public static GameObject MainMenu, OptionsMenu, ControlsMenu, CreditsMenu;
    #endregion

    #region Fun��es Unity
    private void Awake()
    {
        Instance = this;
        Init();
    }
    #endregion

    #region Fun��es Pr�prias
    // Coletando os objetos necess�rios
    private void Init()
    {
        var menuCanvas = GameObject.Find("MainMenu Canvas");
        MainMenu = menuCanvas.transform.Find("MainMenu").gameObject;
        OptionsMenu = menuCanvas.transform.Find("OptionsMenu").gameObject;
        ControlsMenu = menuCanvas.transform.Find("ControlsMenu").gameObject;
        CreditsMenu = menuCanvas.transform.Find("CreditsMenu").gameObject;
    }

    // Abre o menu desejado e fecha o que foi utilizado anteriormente
    public void OpenMenu(Default menu, GameObject callingMenu)
    {
        // Ativando o menu selecionado
        switch (menu)
        {
            case Default.MainMenu:
                MainMenu.SetActive(true);
                break;

            case Default.Options:
                OptionsMenu.SetActive(true);
                break;

            case Default.Controls:
                ControlsMenu.SetActive(true);
                break;

            case Default.Credits:
                CreditsMenu.SetActive(true);
                break;
        }

        // Desativando o anterior
        callingMenu.SetActive(false);
    }
    #endregion
}