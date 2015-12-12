using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PopUp : MonoBehaviour {
    private static PopUp instance;
    public GameObject bannerPrefab;

    private void Awake() { instance = this; }

    public static void DisplayBanner(Sprite pic, string text, Banner.BannerType type) {
        //return instance.banner;

        var go = GameObject.Instantiate(instance.bannerPrefab);
        var banner = go.GetComponent<Banner>();
        banner.CreateBanner(pic, text, type);
        banner.transform.SetParent(instance.transform);
    }

    private void Update() {
        if(Banner.bannerQueue.Count > 0 && !Banner.bannerQueue[0].isShowing) {
            Banner.bannerQueue[0].DisplayBanner();
            Debug.Log("Hete");
        }
    }
}


