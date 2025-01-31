using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : BaseMenu
{
    [Header("References")]
    public AudioMixer mixer;
    public Button backButton;
    [Header("Master")]
    public Slider masterVolSlider;
    public TMP_Text masterVolSliderText;
    [Header("Music")]
    public Slider musicVolSlider;
    public TMP_Text musicVolSliderText;
    [Header("SFX")]
    public Slider sfxVolSlider;
    public TMP_Text sfxVolSliderText;

    public override void InitState(MenuController context)
    {
        base.InitState(context);
        state = MenuController.MenuStates.Settings;

        backButton.onClick.AddListener(JumpBack);

        SetupSliderInformation(masterVolSlider, masterVolSliderText, "MasterVol");
        SetupSliderInformation(musicVolSlider, musicVolSliderText, "MusicVol");
        SetupSliderInformation(sfxVolSlider, sfxVolSliderText, "SFXVol");
    }

    void SetupSliderInformation(Slider mySlider, TMP_Text myText, string parameterName)
    {
        mySlider.onValueChanged.AddListener((value) => OnSliderValueChanged(value, myText, parameterName, mySlider));
        float newVal = (mySlider.value == 0.0f) ? -80.0f : 20.0f * Mathf.Log10(mySlider.value);

        mixer.SetFloat(parameterName, newVal);

        myText.text = (newVal == -80.0f) ? "0%" : (int)(mySlider.value * 10) + "%";
    }

    void OnSliderValueChanged(float value, TMP_Text myText, string parameterName, Slider mySlider)
    {
        value = (value == 0.0f) ? -80.0f : 20.0f * Mathf.Log10(mySlider.value);
        myText.text = (value == -80.0f) ? "0%" : (int)(mySlider.value * 10) + "%";
        mixer.SetFloat(parameterName, value);
    }
}
