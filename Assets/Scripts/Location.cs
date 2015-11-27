using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Location : GameCommandHandler, ISelectable {
    [BitMask(typeof(CharacterStyle))]
    public CharacterStyle clientStyles = CharacterStyle.British;
    public float rotationSpeed;
    public bool rotateLeft;
    public string locationName;
    public List<Package> packages = new List<Package>();
    public Vector2 position { get { return transform.position; } }

    private void Awake() {
        locationName = name;
    }

    private void Update() {
        Rotate();
    }

    public virtual void OnClick() {
        GamePlayer.localInstance.SetSelectedUnits(transform);
    }

    public virtual void SetSelected(bool selected) {

    }

    public override void OnGameTick() {
        base.OnGameTick();
    }

    public virtual void DockWith(Ship ship) {
        ship.LoadPackages(packages);
    }

    private void Rotate() {
        var rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + (rotateLeft ? (rotationSpeed * Time.deltaTime) : -(rotationSpeed * Time.deltaTime)));
    }

    public static Location GetNearestLocation(Vector2 pos) {
        var locations = GameObject.FindObjectsOfType<Location>();
        foreach(var l in locations) {
            if(l.tag != "Hub Station") return l;
        }
        return locations[0];
    }

    public static Vector2[] ToVectorArray(List<Location> locs) {
        var tl = new List<Vector2>();
        foreach(var l in locs) {
            tl.Add(l.position);
        }
        return tl.ToArray();
    }
}