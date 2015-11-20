using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PopUp : MonoBehaviour {
    private static PopUp instance;
    public Banner banner;

    private void Awake() { instance = this; }

    public static Banner DisplayBanner(Sprite pic, string text, Banner.BannerType type) {
        instance.banner.CreateBanner(pic, text, type);
        instance.banner.gameObject.SetActive(true);
        return instance.banner;
    }
}


