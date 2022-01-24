using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsController : MonoBehaviour
{

    [Header("Audio")]
    public Image soundIcon;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    [Header("Facebook")]
    public Image facebookIcon;
    public Sprite facebookLinkedIcon;
    public Sprite notLinkedIcon;
    public Sprite loadingIcon;

    public void Init(SettingsData settingsData){
        UpdateButtonSprite(settingsData.sound);
	if(settingsData.isLinked)
            facebookIcon.sprite = facebookLinkedIcon;
	else
            facebookIcon.sprite = notLinkedIcon;

	AuthManager.Instance().authStateChangedUEvent.AddListener(AuthStateChanged);
    }

    public void Cleanup(){
        AuthManager.Instance().authStateChangedUEvent.RemoveListener(AuthStateChanged);
    }

    public void OnSoundButtonPressed(){
	AudioManager.Instance().ToggleSound();
        SaveManager.Instance.settingsData.sound = AudioManager.Instance().IsOn();
        SaveManager.Instance.SaveGameSettings();
        UpdateButtonSprite(SaveManager.Instance.settingsData.sound);
        AudioManager.Instance().PlayAudio("click_basic");
        AudioManager.Instance().PlayMusic("game_menu");
    }

    public void OnBackButtonPressed(){
        AudioManager.Instance().PlayAudio("click_heavy");
        GameManager.Instance().SwitchState(GameState.MAINMENU);
    }

    public void OnFacebookPressed(){
	if(!SaveManager.Instance.settingsData.isLinked) {
	    AuthManager.Instance().LoginWithFacebook();
	    AudioManager.Instance().PlayAudio("click_basic");	    
	} else {
            AuthManager.Instance().Logout();
	    AudioManager.Instance().PlayAudio("click_heavy");	    	    
        }

	if(AuthManager.Instance().isSigning){
            facebookIcon.sprite = loadingIcon;
        }

    }

    public void UpdateButtonSprite(bool sound){
	if(AudioManager.Instance().IsOn())
 	    soundIcon.sprite = soundOnIcon;
	else
	    soundIcon.sprite = soundOffIcon;
    }

    public void AuthStateChanged(FirebaseUser user){
        Debug.Log("Welcome to the good night!");
        if(user != null)
            facebookIcon.sprite = facebookLinkedIcon;
	else
            facebookIcon.sprite = notLinkedIcon;
    }

}
