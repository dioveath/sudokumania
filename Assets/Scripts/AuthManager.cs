using UnityEngine;
using UnityEngine.Events;
using Firebase.Auth;
using Firebase.Extensions;

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

    void AuthStateChanged(object sender, System.EventArgs eventArgs){
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

	if(isSignedIn)
            Player.Instance.playerData.isLinked = true;
        else
            Player.Instance.playerData.isLinked = false;

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

    void OnDestroy(){
        _auth.StateChanged -= AuthStateChanged;
    }
    

}
