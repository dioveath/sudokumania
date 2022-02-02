using UnityEngine;
using UnityEngine.Events;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{

    public bool isSignedIn = false;
    public bool isSigning = false;
    public UnityEvent<FirebaseUser> authStateChangedUEvent;
    public string username;

    private FirebaseAuth _auth;
    private static AuthManager _instance;


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
                return;
            }

	    if(accessToken == "Cancelled") {
                Debug.LogWarning("Cancelled by User");
                isSigning = false;		
                return;
            }

            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);

            // string profileLink = await FacebookAuthManager.Instance().GetProfileImageLink();
	    // if(profileLink == null){
            //     Debug.LogWarning("Couldn't retrieve profile URL");
            //     return;
            // }

            // Player.Instance.playerData.profileLink = profileLink;
            // Player.Instance.SaveCurrentPlayerData();

            await _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
		isSigning = false;		
        	if(task.IsCanceled) { 
        	    Debug.LogError("SignInWithCredentialAsync was canceled!");
        	    return;
        	}
        	if(task.IsFaulted){
        	    Debug.LogError("SignInWithCredentialAsync encoutered an error: " + task.Exception);
        	    return;
        	}
            });

        } else {
            isSigning = false;
            Debug.LogWarning("Already signed in!");
	}
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs){
        isSignedIn = _auth.CurrentUser != null;

	if(isSignedIn)
	    SaveManager.Instance.settingsData.isLinked = true;
	else
	    SaveManager.Instance.settingsData.isLinked = false;	    

        if(isSignedIn){
            Debug.Log("------SIGNED IN------");
            Debug.Log(_auth.CurrentUser.DisplayName);
            Debug.Log(_auth.CurrentUser.Email);
            Debug.Log(_auth.CurrentUser.IsEmailVerified);
            username = _auth.CurrentUser.DisplayName;
            Debug.Log("------SIGNED IN------");	    
        } else {
            username = "";
        }

        authStateChangedUEvent?.Invoke(_auth.CurrentUser);	
    }

    public void Logout(){
        _auth.SignOut();
    }

    void OnDestroy(){
        _auth.StateChanged -= AuthStateChanged;
    }
    

}
