using UnityEngine;
using UnityEngine.Events;

using Firebase;
using Firebase.Extensions;
using Firebase.Analytics;

public class FirebaseManager : MonoBehaviour
{

    public UnityEvent OnFirebaseInitialized;
    public bool isInitialized = false; 

    private static FirebaseManager _instance;
    public static FirebaseManager Instance(){
	if(_instance == null){
            Debug.LogWarning("Error: No Firebase Manager insantiated yet!");
        }
        return _instance;
    }

    
    void Awake () {
	if(_instance)
            DestroyImmediate(this.gameObject);
        _instance = this;

        OnFirebaseInitialized = new UnityEvent();
    }

    
    void Start(){
        FirebaseInit();
    }


    void FirebaseInit(){
	FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
	    if(task.Exception != null){
		Debug.LogWarning($"Failed to initialize Firebase with { task.Exception }");
		return;
	    }
	    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
	    OnFirebaseInitialized?.Invoke();
	    isInitialized = true;
	    Debug.Log("Firebase Initialized..!");
	});	
    }

}
