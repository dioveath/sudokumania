using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGameplayController : MonoBehaviour
{
    public void OnMenuPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
    }

    public void OnLeaderboardPressed(){
    }
    public void OnSettingsPressed(){
    }
}
