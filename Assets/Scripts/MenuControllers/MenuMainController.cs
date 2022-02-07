using System;
using DG.Tweening;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class MenuMainController : MonoBehaviour
{
    [Header("Facebook")]
    public Image facebookIcon;
    public Sprite facebookLinkedIcon;
    public Sprite notLinkedIcon;
    public Sprite loadingIcon;
    public Text fbLinkText;
    private Tween _loadingTween;
    

    [Header("Sudoku Mania Stats")]
    public Text sudokuPointsText;
    public Text welcomeText;

    [Header("Welcome Dialog")]
    public DialogData welcomeDialogData;

    private void SetSudokuPoints(int points){
	sudokuPointsText.text = $"Your SP: {points}";
    }

    private void SetFacebookStatus(bool isLinked){
	if(isLinked) {
            facebookIcon.sprite = facebookLinkedIcon;
            fbLinkText.text = "FB LINKED";
        } else {
            facebookIcon.sprite = notLinkedIcon;
            fbLinkText.text = "FB NOT LINKED";	    
	}
    }

    private void SetWelcomeMessage(string playerName){
	if(playerName == "") {
	    welcomeText.text = "Welcome, fellow Traveller!";	    
	} else {
	    welcomeText.text = "Welcome, Mr. " + playerName;	    	    
	}
    }

    public void Init(PlayerData data){
	if(data.isFirstTime) {
            welcomeDialogData.Yes = () =>
            {
                DialogManager.Instance.HideDialog();
                Player.Instance.playerData.isFirstTime = false;
                Player.Instance.SaveCurrentPlayerData();
            };
            DialogManager.Instance.ShowDialog(welcomeDialogData);
        }

        SetSudokuPoints(data.points);
        SetFacebookStatus(data.isLinked);
        SetWelcomeMessage(data.playerName);
	
        AuthManager.Instance().authStateChangedUEvent.AddListener(OnAuthStateChanged);
    }

    public void CleanUp(){
        AuthManager.Instance().authStateChangedUEvent.RemoveListener(OnAuthStateChanged);
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

    public void OnFacebookPressed(){
	facebookIcon.sprite = loadingIcon;
	_loadingTween = facebookIcon.transform.DORotate(new Vector3(0, 0, 360), 1.4f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
	_loadingTween.SetAutoKill(false).OnComplete(() => _loadingTween.Restart());	

	if(!Player.Instance.playerData.isLinked) {
	    AudioManager.Instance().PlayAudio("click_basic");	    
	    AuthManager.Instance().LoginWithFacebook();
	} else {
	    AudioManager.Instance().PlayAudio("click_heavy");	    	    
            AuthManager.Instance().Logout();
        }
    }

    public void OnAuthStateChanged(FirebaseUser user){
	if(_loadingTween != null) {
            _loadingTween.OnComplete(() => {
		SetFacebookStatus(user != null);				
	    });
            _loadingTween.SetAutoKill(true);
        } else {
	    SetFacebookStatus(user != null);	    
	}

	if(user != null){
	    SetWelcomeMessage(user.DisplayName);
	} else {
	    SetWelcomeMessage("");	    
	}
    }

}
