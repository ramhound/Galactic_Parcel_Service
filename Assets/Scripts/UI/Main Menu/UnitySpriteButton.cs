using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitySpriteButton : MonoBehaviour {
    public Sprite normal;
    public Sprite selected;
    public Sprite confirmed;
    public Sprite disabled;

    public void OnClick() {
        if(Input.GetMouseButtonUp(0)) GetComponent<SpriteRenderer>().sprite = selected;
    }
}
