using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Banner : MonoBehaviour {
    public enum BannerType { Message, Package }
    public GameObject banner;
    public Image bannerImage;
    public UITweener bannerTweener;
    public float closeAfterDuration = 3f;
    private float closeTime;

    public void CreateBanner(Sprite pic, string text, BannerType type) {
        //profilePic.spriteName = pic;
        //this.text.text = text;
        

        if(type == BannerType.Package) {

        } else {

        }

        bannerImage.sprite = pic;
        bannerTweener.PlayForward();
        closeTime = Time.time + closeAfterDuration;
        enabled = true;
    }

    public void CloseBanner() {
        bannerTweener.PlayReverse();
        bannerTweener.onFinished.Add(new EventDelegate(() => { banner.SetActive(false); }) { oneShot = true });
        enabled = false;
    }

    private void Update() {
        if(Time.time > closeTime)
            CloseBanner();
    }
}
