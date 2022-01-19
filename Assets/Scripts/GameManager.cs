using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;

public class GameManager : MonoBehaviour
{
    // UI objects
    [Header("Menu Parent Holder Objects")]
    public GameObject loginHolder;
    public GameObject mainMenuHolder;
    public GameObject leaderboardHolder;
    public GameObject settingsHolder;
    public GameObject levelSelectHolder;
    public GameObject youwinHolder;
    public GameObject gameplayHolder;
    public GameObject infoHolder;

    // LevelSelect Menu
    [Header("LevelSelect Objects")]
    public GameObject levelButtonHolder;
    public GameObject levelButton;
    public GameObject loadingUI;

    // MainMenu
    [Header("MainMenu Objects")]
    public Text welcomeText;

    private GameObject _lastMenu;
    private static GameManager _instance;
    private GameState _currentState;
    

    void Awake(){
        _instance = this;
    }

    public static GameManager Instance() {
	if(_instance == null) {
            Debug.LogWarning("Error: No GameManager instantiated as of now!");	    
	}
	return _instance;
    }

    void Start() {
        SwitchState(GameState.MAINMENU);
        FirebaseManager.Instance().OnFirebaseInitialized.AddListener(OnFirebaseInitialize);

        AudioManager.Instance().PlayMusic("game_menu");
        // SaveManager.Instance().
    }


    public void SwitchState(GameState state){
        Debug.Log(_currentState + " > " + state);

        // Exit the previous state
        switch(_currentState){
	    case GameState.LOGIN:
                loginHolder.SetActive(false);
                break;
            case GameState.MAINMENU:
                mainMenuHolder.SetActive(false);
                break;
	    case GameState.LEVEL_SELECT:
                levelSelectHolder.SetActive(false);
                break;
	    case GameState.LEADERBOARD:
                leaderboardHolder.SetActive(false);
                break;
            case GameState.GAMEPLAY:
		SudokuManager.Instance().Finish();
                gameplayHolder.SetActive(false);
                break;
            case GameState.YOUWIN:
                youwinHolder.SetActive(false);
                break;
	    case GameState.SETTINGS:
		settingsHolder.SetActive(false);
		break;
	    case GameState.INFO:
                infoHolder.SetActive(false);
                break;

        }

	// Enter the given state
	switch(state){
	    case GameState.LOGIN:
                loginHolder.SetActive(true);
                break;
            case GameState.MAINMENU:
                // welcomeText.text = "Welcome, Mr. " + GameObject.FindWithTag("Player").GetComponent<Player>()._playerData.fullName;
                mainMenuHolder.SetActive(true);
                break;
	    case GameState.LEVEL_SELECT:
                HandleInitializeLevels();
                levelSelectHolder.SetActive(true);
                break;
	    case GameState.LEADERBOARD:
                leaderboardHolder.GetComponent<MenuLeaderboardController>().LoadLeaderboardEntry();
                leaderboardHolder.SetActive(true);
                break;
	    case GameState.SETTINGS:
                settingsHolder.GetComponent<MenuSettingsController>().UpdateButtonSprite(SaveManager.Instance.settingsData.sound);
                settingsHolder.SetActive(true);
		break;
            case GameState.GAMEPLAY:
                // SudokuManager.Instance().Init();
                gameplayHolder.SetActive(true);		
                break;
	    case GameState.YOUWIN:
                youwinHolder.SetActive(true);		
                break;
	    case GameState.INFO:
                infoHolder.SetActive(true);
                break;
        }

        _currentState = state;
    }

    void Update(){
	if(Input.GetKeyDown(KeyCode.Escape)){
	    switch(_currentState){
		case GameState.MAINMENU:
		    Application.Quit();
		    break;
		case GameState.LEVEL_SELECT:
		    SwitchState(GameState.MAINMENU);
		    break;
		case GameState.LEADERBOARD:
		    SwitchState(GameState.MAINMENU);
		    break;
		case GameState.SETTINGS:
		    SwitchState(GameState.MAINMENU);		    
                    break;
		case GameState.INFO:
                    SwitchState(GameState.MAINMENU);
                    break;
            }
	}
    }

    // this should be moved to MenuLevelSelectcontroller
    async void HandleInitializeLevels(){
        SudokuUtils.onLevelsLoaded.AddListener(OnLevelsLoadedCallback);
        loadingUI.SetActive(true);

	if(FirebaseManager.Instance().isInitialzed) {
	    await SudokuUtils.GetAllLevels(FirebaseDatabase.DefaultInstance);
	} else {
            await SudokuUtils.GetAllLevels(null);
        }

        int levelsCount = SudokuUtils.allSudokuLevels.Count;
        Button[] currentButtons = levelButtonHolder.GetComponentsInChildren<Button>();
        int currentTotalButtons = currentButtons.Length;

        for (int i = 0; i < levelsCount; i++){
            GameObject buttonObj;
            if(i >= currentTotalButtons) {
		buttonObj = Instantiate(levelButton, Vector3.zero, Quaternion.identity);
	    } else {
                buttonObj = currentButtons[i].gameObject;
            }

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.SetParent(levelButtonHolder.transform);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
	    rect.anchoredPosition3D = new Vector3(0, -900 -(i * 400), 0);
	    rect.localScale = Vector3.one;

            int levelIndex = i+1;
            buttonObj.GetComponentInChildren<Text>().text = "Level " + levelIndex;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => {
                SwitchState(GameState.GAMEPLAY);
		SudokuManager.Instance().Init(levelIndex);
                AudioManager.Instance().PlayAudio("click_wooden1");
            });
        }

	if(currentTotalButtons > levelsCount){
	    Debug.Log("Destroying..!");
            for (int i = levelsCount; i < currentTotalButtons; i++){
                Destroy(currentButtons[i]);
            }
	}
    }

    public void OnLevelsLoadedCallback(){
        Debug.Log("loadingUI");
        loadingUI.SetActive(false);
        SudokuUtils.onLevelsLoaded.RemoveListener(OnLevelsLoadedCallback);
    }

    public void OnFirebaseInitialize(){
        AuthManager.Instance().authStateChangedUEvent.AddListener(OnAuthStateChange);	
    }
    
    public void OnAuthStateChange(FirebaseUser user){
	// NOTE: We're not using any authentication system for initial release
        // if(user != null){
        //     SwitchState(GameState.MAINMENU);
	//     Debug.Log("Game logged in with: " + user.DisplayName);
        // } else {
        //     SwitchState(GameState.LOGIN);
        // }	
    }

    
}




[Serializable]
public enum GameState
{
    LOGIN,
    MAINMENU,
    LEADERBOARD,
    SETTINGS,
    LEVEL_SELECT,
    INFO,
    YOUWIN,
    GAMEPLAY
};
