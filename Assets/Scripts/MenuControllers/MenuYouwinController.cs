using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuYouwinController : MonoBehaviour
{
    public void OnMainMenuPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
    }
    public void OnRetryPressed(){
        SudokuManager.Instance().ReloadLevel();
        GameManager.Instance().SwitchState(GameState.GAMEPLAY);
    }
}
