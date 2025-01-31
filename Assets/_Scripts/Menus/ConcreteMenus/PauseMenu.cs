using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : BaseMenu
{
    public Button resumeGame;
    public Button returnToMenu;
    public Button quitGame;
    public AudioSource music;

    public override void InitState(MenuController context)
    {
        base.InitState(context);
        state = MenuController.MenuStates.Pause;

        resumeGame.onClick.AddListener(() => SetNextMenu(MenuController.MenuStates.InGame));
        returnToMenu.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
        quitGame.onClick.AddListener(QuitGame);
    }

    public override void EnterState()
    {
        base.EnterState();
        music.Pause();
        Time.timeScale = 0.0f;
    }

    public override void ExitState()
    {
        base.ExitState();
        music.Play();
        Time.timeScale = 1.0f;
    }

    public void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
