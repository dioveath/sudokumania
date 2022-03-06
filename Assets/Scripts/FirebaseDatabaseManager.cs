using UnityEngine;
using Firebase.Database;

class FirebaseDatabaseManager : MonoBehaviour
{

    public FirebaseDatabase db;
    // private bool isInitialized = false;

    private static FirebaseDatabaseManager _instance;
    public static FirebaseDatabaseManager Instance
    {
	get {
	    if(_instance == null) {
		Debug.LogError("No Instantiation FirebaseDatabaseManager Script");
		return null;
	    }
            return _instance;
        }
    }

    void Awake(){
	if(_instance != null){
            DestroyImmediate(this.gameObject);
        }
        _instance = this;
    }

    void Start(){
	FirebaseManager.Instance().OnFirebaseInitialized.AddListener(FirebaseInitializedCallback);
    }

    void FirebaseInitializedCallback(){
        db = FirebaseDatabase.DefaultInstance;
        // isInitialized = true;
    }



}
