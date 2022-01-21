using UnityEngine;
using UnityEngine.UI;

public class MenuYouwinController : MonoBehaviour
{
    public Text pointsText;

    public void SetCompletedStats(){
        float elapsedSeconds = SudokuManager.Instance().GetTimeElapsed();
        int minutes = (int) (elapsedSeconds / 60);
	int seconds = (int) (elapsedSeconds % 60);
        int points = SudokuManager.Instance().GetCurrentLevelPoint();

        float percent = 0.0f;
        int playerPoint = 0;
        if(elapsedSeconds <= 300){
            percent = elapsedSeconds / 300;
	    if((1 - percent) > 0.75f) percent = 0.25f;
            playerPoint = points + (int)(points * (1 - percent));
        } else {
            percent = (elapsedSeconds / 300) * 0.5f;
            playerPoint = points - (int)(points * (1 - percent));
        }
        pointsText.text = $"{minutes.ToString("00")} : {seconds.ToString("00")} \n{playerPoint}";	

	Player.Instance.playerData.points += playerPoint;
        Player.Instance.SaveCurrentPlayerData();
    }

    public void OnMainMenuPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
    }
    public void OnRetryPressed(){
        SudokuManager.Instance().ReloadLevel();
        GameManager.Instance().SwitchState(GameState.GAMEPLAY);
    }

}
