using UnityEngine;
using UnityEngine.UI;

public class MenuYouwinController : MonoBehaviour
{
    public Text pointsText;

    public async void SetCompletedStats(){
        SudokuLevel wonLevel = SudokuManager.Instance().GetCurrentSudokuLevel();
        SudokuLevel playerSavedLevel = Player.Instance.GetPlayingSudokuLevel(wonLevel.id);

        int playerPoint = 0;

	float elapsedSeconds = SudokuManager.Instance().GetTimeElapsed();
	int minutes = (int) (elapsedSeconds / 60);
	int seconds = (int) (elapsedSeconds % 60);
	int points = wonLevel.points;	
	// if player hasn't played this before, give them full points or only 100 points
	// TODO: change the 100 points to something that decreases gradually
	if(playerSavedLevel == null || (playerSavedLevel != null ? !playerSavedLevel.isCompleted : false)) { 
	    float percent = 0.0f;
	    if(elapsedSeconds <= 300){
		percent = elapsedSeconds / 300;
		if((1 - percent) > 0.75f) percent = 0.25f;
		playerPoint = points + (int)(points * (1 - percent));
	    } else {
		percent = (elapsedSeconds / 300) * 0.5f;
		playerPoint = points - (int)(points * (1 - percent));
	    }
	} else {
            playerPoint += 100;
        }
	pointsText.text = $"{minutes.ToString("00")} : {seconds.ToString("00")} \n{playerPoint}";		    

	Player.Instance.playerData.points += playerPoint;
        wonLevel.isCompleted = true;
        wonLevel.Reset();
        Player.Instance.playerData.lastPlayedId = "";
        Player.Instance.AddPlayingSudokuLevel(wonLevel);

	if(AuthManager.Instance().isSignedIn){
            string username = AuthManager.Instance().username;
            LeaderboardManager.Instance.EnterNewLeaderboardEntry(
		username,
		Player.Instance.playerData.points,
		Player.Instance.playerData.profileLink
	    );

            Debug.Log("Saved online!");
        } else {
            Debug.Log("Only Saved locally!");
        }


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
