using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("Button")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public Button backButton;
    public Button returnToMenu;
    public Button returnToGame;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject pauseMenu;

    [Header("Text")]
    public TMP_Text volSliderText;
    public TMP_Text livesText;

    [Header("Sliders")]
    public Slider volSlider;

    public static bool IsGamePaused { get; private set; } = false; // Global pause state

    // Start is called before the first frame update
    void Start()
    {
        //Button Bindings
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
        if (playButton) playButton.onClick.AddListener(() => SceneManager.LoadScene("Level1"));
        if (settingsButton) settingsButton.onClick.AddListener(() => SetMenus(settingsMenu, mainMenu));
        if (backButton) backButton.onClick.AddListener(() => SetMenus(mainMenu, settingsMenu));
        if (returnToMenu) returnToMenu.onClick.AddListener(() => { SceneManager.LoadScene("Menu"); ResumeGame(); });
        if (returnToGame) returnToGame.onClick.AddListener(() => { SetMenus(null, pauseMenu); ResumeGame(); });


        //init menu states
        if (mainMenu)
            mainMenu.SetActive(true);
        if (settingsMenu)
            settingsMenu.SetActive(false);
        if (pauseMenu)
            pauseMenu.SetActive(false);
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void SetMenus(GameObject menuToActivate, GameObject menuToDeactivate)
    {
        if (menuToActivate)
            menuToActivate.SetActive(true);
        if (menuToDeactivate)
            menuToDeactivate.SetActive(false);
    }

    void OnSliderValueChanged(float sliderValue)
    {
        if (volSliderText)
            volSliderText.text = volSlider.value.ToString();
    }

    private void Update()
    {
        if (!pauseMenu) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            if (pauseMenu.activeSelf)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0; // Pause all time-based processes
        IsGamePaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resume all time-based processes
        IsGamePaused = false;
    }
}
