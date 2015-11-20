using UnityEngine;
using System.Collections;

public class PopUp : MonoBehaviour {
    private static PopUp instance;
    public Banner banner;

    private void Awake() { instance = this; }

    public static Banner DisplayBanner(Sprite pic, string text, Banner.BannerType type, bool fail = false) {
        instance.banner.CreateBanner(pic, text, type, fail);
        instance.banner.gameObject.SetActive(true);
        return instance.banner;
    }
}


