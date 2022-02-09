#define DEBUG

using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;

    private static AdsManager _instance;
    public static AdsManager Instance {
        get
        {
	    if(_instance == null) {
                Debug.LogError("No AdsManager Script attached");
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
        MobileAds.Initialize(initStatus => { });

#if DEBUG
        string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
        string bannerAdUnitId = "ca-app-pub-4040965046942345/3366033396";
#endif
        _bannerView = new BannerView(bannerAdUnitId, AdSize.IABBanner, AdPosition.Bottom);


#if DEBUG
        string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
#else
        string interstitialAdUnitId = "ca-app-pub-4040965046942345/8339675391";
#endif

        _interstitialAd = new InterstitialAd(interstitialAdUnitId);
        _interstitialAd.OnAdLoaded += (object sender, EventArgs args) => { Debug.Log("Interstitial Ad Loaded");  };

        RequestBanner();
    }

    void RequestBanner(){
        AdRequest request = new AdRequest.Builder().Build();
        // _bannerView.LoadAd(request);
    }

    public void RequestInterstitial(){
        AdRequest request = new AdRequest.Builder().Build();
        _interstitialAd.LoadAd(request);
    }

    public void ShowInterstitial(){
	if(_interstitialAd.IsLoaded()){
            // _interstitialAd.Show();
        }
    }

}
