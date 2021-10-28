using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLoginPageController : MonoBehaviour
{

    public void OnFacebookLoginButtonClicked(){
        AuthManager.Instance().LoginWithFacebook();
    }

    public void OnAnonymousLoginButtonClicked(){}

    public void OnGoogleLoginButtonClicked(){}


}
