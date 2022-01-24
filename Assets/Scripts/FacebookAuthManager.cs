using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System.Threading.Tasks;

public class FacebookAuthManager : MonoBehaviour
{

    private static FacebookAuthManager _instance;

    public static FacebookAuthManager Instance(){
        return _instance;
    }

    void Awake(){
        if(_instance != null)
            DestroyImmediate(this.gameObject);
        _instance = this;

        if(!FB.IsInitialized){
            FB.Init(InitCallback, OnHideUnity);
        } else { 
            FB.ActivateApp();
        }
    }

    private void InitCallback(){
        if(FB.IsInitialized){
            FB.ActivateApp();
        } else { 
            Debug.LogWarning("Failed to Initialize the Facebook SDK!");
        }
    }

    private void OnHideUnity(bool isGameShown){
        if(!isGameShown){
            Time.timeScale = 0;
        } else { 
            Time.timeScale = 1;
        }
    }


    // Gets FB Token by Logging
    public Task<string> LogInAsync () {
        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, result => {
	    if(result.Error != null) {
                Debug.LogWarning(result.Error);
                tcs.SetResult("Error");
                return;
            }
            tcs.SetResult(result.AccessToken.TokenString);
        });
	return tcs.Task;
    }

    public void LogIn(FacebookDelegate<ILoginResult> authCallback){        
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, authCallback);
    }

    private void AuthCallback(ILoginResult result){
        if(FB.IsLoggedIn){
            AccessToken accessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log(accessToken.UserId + " Logged In from Facebook.");

            foreach(string perm in accessToken.Permissions){
                Debug.Log(perm);
            }

        } else { 
            Debug.Log("User cancelled login!");
        }
    }


    private void LoginStatusCallback(ILoginStatusResult result){
        if(!string.IsNullOrEmpty(result.Error)){
            Debug.Log("Error: " + result.Error);
        } else if(result.Failed){
            Debug.Log("Failure: Access Token could not be retrieved!");
        } else { 
            Debug.Log("Success: " + result.AccessToken.UserId);
        }
    }

    
}
