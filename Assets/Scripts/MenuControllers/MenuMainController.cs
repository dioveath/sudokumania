using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMainController : MonoBehaviour
{

    public void OnPlayButtonPressed(){
        AudioManager.Instance().PlayAudio("click_basic");
        GameManager.Instance().SwitchState(GameState.LEVEL_SELECT);
    }

    public void OnSettingsButtonPressed(){
	AudioManager.Instance().PlayAudio("click_basic");
	GameManager.Instance().SwitchState(GameState.SETTINGS);
    }

    public void OnLeaderButtonPressed(){
        AudioManager.Instance().PlayAudio("click_basic");
        GameManager.Instance().SwitchState(GameState.LEADERBOARD);
    }

    public void OnLogoutButtonPressed(){
        AudioManager.Instance().PlayAudio("click_heavy");	
        AuthManager.Instance().Logout();
    }

    public void OnInfoButtonPressed(){
        AudioManager.Instance().PlayAudio("click_basic");
        GameManager.Instance().SwitchState(GameState.INFO);
    }

}
