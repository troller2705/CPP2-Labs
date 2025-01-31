using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameMenu : BaseMenu
{
    public TMP_Text livesText;

    public override void InitState(MenuController context)
    {
        base.InitState(context);
        state = MenuController.MenuStates.InGame;

        //livesText.text = $"Lives: {GameManager.Instance.lives}";

        //GameManager.Instance.OnLifeValueChanged += OnLifeValueChanged;
    }

    private void OnLifeValueChanged(int lives)
    {
        //livesText.text = $"Lives: {GameManager.Instance.lives}";
    }

    private void OnDestroy()
    {
        //GameManager.Instance.OnLifeValueChanged -= OnLifeValueChanged;
    }
}
