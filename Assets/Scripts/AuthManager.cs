using UnityEngine;
using UnityEngine.Events;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{

    public bool isSignedIn = false;
    public bool isSigning = false;

    public UnityEvent<FirebaseUser> authStateChangedUEvent;

    private FirebaseAuth _auth;
    private static AuthManager _instance;

    [SerializeField]
    private DialogData loginErrorDialogData;

    public static AuthManager Instance(){
        return _instance;
    }

    void Awake(){
        if(_instance != null)
            DestroyImmediate(this.gameObject);
        _instance = this;

        authStateChangedUEvent = new UnityEvent<FirebaseUser>();

        FirebaseManager.Instance().OnFirebaseInitialized.AddListener(FirebaseInitializedCallback);
    }

    void FirebaseInitializedCallback(){
        _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;
    }

    public async void LoginWithFacebook(){
        isSigning = true;
        if(!isSignedIn) { 
            string accessToken = await FacebookAuthManager.Instance().LogInAsync();
	    if(accessToken == "Error"){
                Debug.LogWarning("Access Token Error");
                isSigning = false;
                loginErrorDialogData.headerText = "LOGIN ERROR";
                loginErrorDialogData.bodyText = "There was some problem while logging in!";
                DialogManager.Instance.ShowDialog(loginErrorDialogData);
                authStateChangedUEvent?.Invoke(null);		
                return;
            }

	    if(accessToken == "Cancelled") {
		Debug.LogWarning("Cancelled by User");
                loginErrorDialogData.headerText = "LOGIN CANCELLED";
                loginErrorDialogData.bodyText = "User cancelled the login!";
                DialogManager.Instance.ShowDialog(loginErrorDialogData);		
                isSigning = false;
                authStateChangedUEvent?.Invoke(null);
                return;
            }

            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);

            string profileLink = await FacebookAuthManager.Instance().GetProfileImageLink();
	    if(profileLink == null){
                Debug.LogWarning("Couldn't retrieve profile URL");
                loginErrorDialogData.headerText = "LOGIN ERROR";
                loginErrorDialogData.bodyText = "Couldn't retrive Profile Picture of the User";
                DialogManager.Instance.ShowDialog(loginErrorDialogData);
                FacebookAuthManager.Instance().LogOut();
                authStateChangedUEvent?.Invoke(null);		
                return;
            }

            Player.Instance.playerData.profileLink = profileLink;
            Player.Instance.SaveCurrentPlayerData();

            await _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
		isSigning = false;		
        	if(task.IsCanceled) { 
		    loginErrorDialogData.headerText = "LOGIN CANCELLED";
                    loginErrorDialogData.bodyText = "Login cancelled by the user!";
                    DialogManager.Instance.ShowDialog(loginErrorDialogData);		    
                    Debug.LogError("SignInWithCredentialAsync was canceled!");
		    authStateChangedUEvent?.Invoke(null);		    
        	    return;
        	}
        	if(task.IsFaulted){
		    loginErrorDialogData.headerText = "LOGIN ERROR";
                    loginErrorDialogData.bodyText = "Error: " + task.Exception;
                    DialogManager.Instance.ShowDialog(loginErrorDialogData);  		    
        	    Debug.LogError("SignInWithCredentialAsync encoutered an error: " + task.Exception);
		    authStateChangedUEvent?.Invoke(null);		    
        	    return;
        	}

		loginErrorDialogData.headerText = "LOGIN SUCCESSFUL";		
		loginErrorDialogData.bodyText = "Logged in with " + task.Result.DisplayName + "!";
		DialogManager.Instance.ShowDialog(loginErrorDialogData);	    		
            });

        } else {
            Debug.LogWarning("Already signed in!");
	    loginErrorDialogData.headerText = "LOGIN ERROR";
	    loginErrorDialogData.bodyText = "Already signed in!";
	    DialogManager.Instance.ShowDialog(loginErrorDialogData);	    
            isSigning = false;
	}
    }

    async void AuthStateChanged(object sender, System.EventArgs eventArgs){
        isSignedIn = _auth.CurrentUser != null;

        if(isSignedIn){
            Debug.Log("------SIGNED IN------");
            Debug.Log(_auth.CurrentUser.UserId);
            Debug.Log(_auth.CurrentUser.DisplayName);
            Debug.Log(_auth.CurrentUser.Email);
            Debug.Log(_auth.CurrentUser.IsEmailVerified);
            Player.Instance.playerData.playerName = _auth.CurrentUser.DisplayName;
            Debug.Log("------SIGNED IN------");	    
        } else {
            Player.Instance.playerData.playerName = "";
        }

	if(isSignedIn) {
            Player.Instance.playerData.isLinked = true;
	    PlayerData onlinePlayerData = await GetPlayerDataFromDB(_auth.CurrentUser.UserId);
	    if(onlinePlayerData == null) {
                onlinePlayerData = new PlayerData();
                onlinePlayerData = Player.Instance.playerData;
                onlinePlayerData.userId = _auth.CurrentUser.UserId;
                onlinePlayerData.email = _auth.CurrentUser.Email;
                onlinePlayerData.fullName = _auth.CurrentUser.DisplayName;
                SavePlayerToDB(onlinePlayerData);
	    } else {
		Player.Instance.playerData = onlinePlayerData;
	    }	    
	} else {
            Player.Instance.playerData = new PlayerData();
        }

        Player.Instance.SaveCurrentPlayerData();
        authStateChangedUEvent?.Invoke(_auth.CurrentUser);
    }

    public void Logout(){
        _auth.SignOut();
        FacebookAuthManager.Instance().LogOut();	
	loginErrorDialogData.headerText = "LOGOUT SUCCESSFUL";
        loginErrorDialogData.bodyText = "Logged out!";
        DialogManager.Instance.ShowDialog(loginErrorDialogData);
    }

    public void SavePlayerToDB(PlayerData pd){
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        for(int i = 0; i < pd.playingLevels.Count; i++){
            pd.playingLevels[i].sudokuArrayString = SudokuUtils.PuzzleToString(pd.playingLevels[i].sudokuArray);
            pd.playingLevels[i].inputSudokuArrayString = SudokuUtils.PuzzleToString(pd.playingLevels[i].inputSudokuArray);
            pd.playingLevels[i].validSolutionString = SudokuUtils.PuzzleToString(pd.playingLevels[i].validSolution);	    	    
        }
	string json = JsonUtility.ToJson(pd);
        dbRef.Child("players").Child(pd.userId).SetRawJsonValueAsync(json).ContinueWith(task => {
	    if(task.IsFaulted){
                Debug.LogError("Error: " + task.Exception);
            }
	    if(task.IsCompleted) {
                Debug.Log("Player Data saved!");
            }
	});
    }

    public async Task<PlayerData> GetPlayerDataFromDB(string userId){
        DataSnapshot snapshot = await FirebaseDatabase.DefaultInstance.GetReference("players/" + userId).GetValueAsync();
	if(!snapshot.Exists) return null;

        string json = snapshot.GetRawJsonValue();
	PlayerData pd = JsonUtility.FromJson<PlayerData>(json);

        for(int i = 0; i < pd.playingLevels.Count; i++){
            pd.playingLevels[i].sudokuArray = SudokuUtils.StringToPuzzle(pd.playingLevels[i].sudokuArrayString);
            pd.playingLevels[i].inputSudokuArray = SudokuUtils.StringToPuzzle(pd.playingLevels[i].inputSudokuArrayString);
            pd.playingLevels[i].validSolution = SudokuUtils.StringToPuzzle(pd.playingLevels[i].validSolutionString);	    	    
        }	

        return pd;
    }    

    void OnDestroy(){
        _auth.StateChanged -= AuthStateChanged;
    }
    

}
