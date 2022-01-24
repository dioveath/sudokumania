using UnityEngine;
using UnityEngine.Events;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{

    public bool isSignedIn = false;
    public bool isSigning = false;
    public UnityEvent<FirebaseUser> authStateChangedUEvent;

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
                return;
            }
            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
            await _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
        	if(task.IsCanceled) { 
        	    Debug.LogError("SignInWithCredentialAsync was canceled!");
        	    return;
        	}
        	if(task.IsFaulted){
        	    Debug.LogError("SignInWithCredentialAsync encoutered an error: " + task.Exception);
        	    return;
        	}
                isSigning = false;
            });
        } else {
            isSigning = false;
            Debug.LogWarning("Already signed in!");
	}
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs){
        isSignedIn = _auth.CurrentUser != null;
        authStateChangedUEvent?.Invoke(_auth.CurrentUser);

	if(isSignedIn)
	    SaveManager.Instance.settingsData.isLinked = true;
	else
	    SaveManager.Instance.settingsData.isLinked = false;	    

        if(isSignedIn){ 
            Debug.Log(_auth.CurrentUser.Email);
            Debug.Log(_auth.CurrentUser.DisplayName);
            Debug.Log(_auth.CurrentUser.IsEmailVerified);		
        }
    }

    public void Logout(){
        _auth.SignOut();
    }

    void OnDestroy(){
        _auth.StateChanged -= AuthStateChanged;
    }
    

}
