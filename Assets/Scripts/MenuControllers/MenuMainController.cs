using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMainController : MonoBehaviour
{

    public void OnPlayButtonPressed(){
        GameManager.Instance().SwitchState(GameState.LEVEL_SELECT);
    }

    public void OnLeaderButtonPressed(){
        GameManager.Instance().SwitchState(GameState.LEADERBOARD);
    }

    public void OnLogoutButtonPressed(){
        AuthManager.Instance().Logout();
    }

}
