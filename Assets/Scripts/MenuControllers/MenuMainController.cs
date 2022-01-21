using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuMainController : MonoBehaviour
{

    public Text sudokuPointsText;

    void Start(){
    }

    public void SetSudokuPoints(){
	int points = Player.Instance.playerData.points;
	sudokuPointsText.text = $"Your SP: {points}";
	Debug.Log(Player.Instance.playerData.points);
	Debug.Log(points);		    
    }

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
