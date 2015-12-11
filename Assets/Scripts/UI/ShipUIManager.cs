using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShipUIManager : MonoBehaviour {
    public Ship selectedShip;
    public Text currentCmdLabel;
    public Text cargoLabel;
    public GameObject distanceMarkerPrefab;
    public Transform markerParent;
    public static List<DistanceMarker> markers = new List<DistanceMarker>();
    public static bool showing = false;

    private void OnEnable() {
        CreateDistanceMarkers();
        showing = true;
    }

    private void OnDisable() {
        RemoveAllMarkers();
        showing = false;
    }

    private void Update() {
        currentCmdLabel.text = "Command: " + selectedShip.currentCommand.ToString();
        cargoLabel.text = "Cargo Count: " + selectedShip.cargo.Count;
    }

    public void CreateDistanceMarkers() {
        RemoveAllMarkers();
        foreach(var kv in selectedShip.cargo) {
            markers.Add(CreateDistanceMarker(kv.Key, kv.Value.Count));
        }
    }

    private void RemoveAllMarkers() {
        foreach(var m in markers)
            GameObject.Destroy(m.gameObject);
        markers.Clear();
    }

    public void RemoveMarkerFor(Location loc) {
        DistanceMarker marker = null;
        foreach(var m in markers) {
            if(m.loc == loc) {
                GameObject.Destroy(m.gameObject);
                marker = m;
            }
        }

        if(marker != null)
            markers.Remove(marker);
    }

    private DistanceMarker CreateDistanceMarker(Location loc, int packageCount) {
        var go = Instantiate(distanceMarkerPrefab) as GameObject;
        var distMarker = go.GetComponent<DistanceMarker>();
        distMarker.transform.SetParent(markerParent);
        distMarker.transform.localPosition = Vector2.zero;
        distMarker.loc = loc;
        distMarker.packageCountLabel.text = "" + packageCount;
        distMarker.selectedShip = selectedShip;
        return distMarker;
    }
}
