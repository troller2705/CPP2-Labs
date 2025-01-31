using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMenu : MonoBehaviour
{
    [HideInInspector]
    public MenuController.MenuStates state;

    protected MenuController context;

    public virtual void InitState(MenuController context)
    {
        this.context = context;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void JumpBack()
    {
        context.JumpBack();
    }

    public void SetNextMenu(MenuController.MenuStates newState)
    {
        context.SetActiveState(newState);
    }
}