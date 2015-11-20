using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using GoogleMobileAds.Api;

public class PixelAnalytics : MonoBehaviour {
    private static PixelAnalytics instance;
    public static Dictionary<string, object> DataModel { get; private set; }
    public static float TotalAppTime { get; set; }
    public static float TotalPlayTime { get; set; }
    public static float CurrentPlayTime { get; set; }
    //public static BannerView bannerView { get; set; }
    //public static AdRequest adRequest;

    private bool countAppTime;
    private bool countPlayTime;
    private bool countCurrentPlayTime;

    public delegate void PlayTimerTick();
    private static event PlayTimerTick _onTimerTick;
    public static event PlayTimerTick onTimerTick {
        add { _onTimerTick += value; }
        remove { _onTimerTick -= value; }
    }

    static PixelAnalytics() {
        DataModel = new Dictionary<string, object>();
    }

    private void Awake() {
        instance = this;

        //bannerView = new BannerView(
        //        "ca-app-pub-3772989464406088/1397530254", AdSize.Banner, AdPosition.Top);
        //adRequest = new AdRequest.Builder().
        //    AddTestDevice(AdRequest.TestDeviceSimulator).
        //    AddTestDevice("482FFD77CFAD109274EB9F13FE6FC98D").
        //    Build();
    }

    private void Update() {
        TotalAppTime += Time.deltaTime;
        if(countCurrentPlayTime) CurrentPlayTime += Time.deltaTime;
    }

    public static void StartPlayTimer() {
        instance.countCurrentPlayTime = true;
    }

    public static void PausePlayTimer() {
        instance.countCurrentPlayTime = false;
    }

    public static void Save() {
        PlayerPrefs.SetFloat("TotalAppTime", PixelAnalytics.TotalAppTime);
        PlayerPrefs.Save();
    }
}
