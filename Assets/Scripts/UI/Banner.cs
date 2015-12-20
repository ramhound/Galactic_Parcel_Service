using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Banner : MonoBehaviour {
    public enum BannerType { Message, Package }
    public GameObject banner;
    public Image bannerImage;
    public Text bannerText;
    public UITweener bannerTweener;
    public static List<Banner> bannerQueue = new List<Banner>();
    public float closeAfterDuration = 3f;
    private float closeTime;
    public bool isShowing = false;

    public void CreateBanner(Sprite pic, string text, BannerType type) { 
        bannerImage.sprite = pic;
        bannerText.text = text;
        Banner.bannerQueue.Add(this);
    }

    public void DisplayBanner() {
        closeTime = Time.time + closeAfterDuration;
        gameObject.SetActive(true);
        bannerTweener.PlayForward();
        isShowing = true;
    }

    public void CloseBanner() {
        bannerTweener.PlayReverse();
        bannerTweener.onFinished.Add(new EventDelegate(()
            => {
                banner.SetActive(false);
                Destroy(gameObject);
                isShowing = false;
                Banner.bannerQueue.Remove(this);
            }) { oneShot = true });
    }

    private void Update() {
        if(Time.time > closeTime)
            CloseBanner();
    }
}
