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
    public static List<Location> discoveredLocations = new List<Location>();
    public List<HubStation> shipingFacilities = new List<HubStation>();
    public List<Package> packages = new List<Package>();
    public Vector2 position { get { return transform.position; } }

    public override void Awake() {
        base.Awake();
        locationName = name;
    }

    public override void Start() {
        base.Start();
        if(!discoveredLocations.Contains(this))
            discoveredLocations.Add(this);
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
        if(ship.currentCommand == GameCommand.Delivery) {
            for(int i = ship.cargo.Count - 1; i >= 0; i--) {
                if(ship.cargo[i].receiver.location == this) {
                    ship.cargo[i].receiver.PackageDelivered();
                    ship.cargo.RemoveAt(i);
                }
            }
        }
    }

    public virtual void LoadPackages(Ship ship) { }

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

    public virtual void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Delivery Zone Collider") {
            var sf = col.GetComponentInParent<HubStation>();
            if(!shipingFacilities.Contains(sf))
                shipingFacilities.Add(sf);
        }
    }
}