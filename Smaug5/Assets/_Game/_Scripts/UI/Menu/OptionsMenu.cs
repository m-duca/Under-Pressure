using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    #region Vari�veis Globais
    [Header("Configura��es:")]
    // Inspector:
    [Header("Refer�ncias:")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    #endregion

    #region Fun��es Unity
    private void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
            LoadVolume();
        else
            SetMasterVolume();
    }
    #endregion

    #region Fun��es Pr�prias
    public void SetMasterVolume()
    {
        float volume = volumeSlider.value;
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    private void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume");

        SetMasterVolume();
    }

    public void GoBackToMainMenu() => MainMenuManager.Instance.OpenMenu(Default.MainMenu, MainMenuManager.OptionsMenu);

    public void GoBackToGameMenu() => GameMenuManager.Instance.OpenMenu(InGame.GameMenu, GameMenuManager.OptionsMenu);
    #endregion
}
