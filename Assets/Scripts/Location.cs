using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Location : PlayerCommandHandler, ISelectable {
    [BitMask(typeof(CharacterStyle))]
    public CharacterStyle clientStyles = CharacterStyle.British;
    private Transform trans;
    private SpriteRenderer rend;
    public float rotationSpeed;
    public bool rotateLeft;
    public List<Client> clients = new List<Client>();

    private void Awake() {
        trans = transform;
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update() {
        Rotate();
    }

    public virtual void OnClick() {
        SetSelected(true);
    }

    public virtual void SetSelected(bool selected) {
        if(selected) {
            GamePlayer.localInstance.SetSelectedUnit(this);
            Camera.main.GetComponent<CameraFollow>().SetMainTarget(trans);
        }
    }

    public override void OnGameTick() {
        base.OnGameTick();

        GamePlayer.localInstance.DisplayBanner(new Vector2(-1, 0), "test", Banner.BannerType.Message);
    }

    private void Rotate() {
        var rot = trans.rotation.eulerAngles;
        trans.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + (rotateLeft ? (rotationSpeed * Time.deltaTime) : -(rotationSpeed * Time.deltaTime)));
    }
}