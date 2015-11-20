using UnityEngine;
using System.Collections;
//using GoogleMobileAds;
//using Opencoding.CommandHandlerSystem;

public class BannerAdEventHandler : MonoBehaviour {
    //private static BannerAdEventHandler instance;
    //public static string adKey = "adkey";
    //public TweenPosition headerUiTween;
    //public float delaybeforeBannerAd = 3;
    //private bool adLoaded = false;

    private void Start() {
        //instance = this;

        //if(PlayerPrefs.HasKey(BannerAdEventHandler.adKey) && PlayerPrefs.GetInt(BannerAdEventHandler.adKey) == 1) {
        //    CommandHandlers.RegisterCommandHandlers(typeof(BannerAdEventHandler));
        //    SetupBannerAdCallbacks();
        //}
    }

    //private void Update() {
    //    if(PixelAnalytics.CurrentPlayTime > delaybeforeBannerAd * 60 && !adLoaded) {
    //        if(PixelAnalytics.adRequest != null) {
    //            PixelAnalytics.bannerView.LoadAd(PixelAnalytics.adRequest);
    //            adLoaded = true;
    //        }
    //    }
    //}

    //private void SetupBannerAdCallbacks() {
    //    PixelAnalytics.bannerView.AdLoaded += (s, a) => {
    //        headerUiTween.Play(true);
    //    };

    //    PixelAnalytics.bannerView.AdOpened += (s, a) => {
    //        PixelAnalytics.bannerView.Hide();
    //        headerUiTween.Play(false);
    //    };

    //    PixelAnalytics.bannerView.AdFailedToLoad += (s, a) => {
    //        PixelAnalytics.adRequest = null;
    //    };
    //}

    //[CommandHandler(Name = "HideAds")]
    //private static void HideAds() {
    //    PixelAnalytics.bannerView.Hide();
    //    instance.headerUiTween.Play(false);
    //}
}
