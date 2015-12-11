using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipUIManager : MonoBehaviour {
    public Ship selectedShip;
    public Text currentCmdLabel;
    public Text cargoLabel;
    public GameObject distMarkerPrefab;

    private void OnEnable() {
        CreateDistanceMarkers();
    }

    private void Update() {
        currentCmdLabel.text = "Command: " + selectedShip.currentCommand.ToString();
        cargoLabel.text = "Cargo Count: " + selectedShip.cargo.Count;
    }

    public void CreateDistanceMarkers() {
        foreach(var kv in selectedShip.cargo) {
            CreateDistanceMarker(kv.Key, kv.Value.Count);
        }
    }

    private DistanceMarker CreateDistanceMarker(Location loc, int packageCount) {
        var distmarker = Instantiate(distMarkerPrefab).GetComponent<DistanceMarker>();
        distmarker.transform.SetParent(transform);
        distmarker.loc = loc;
        distmarker.packageCountLabel.text = "" + packageCount;
        return distmarker;
    }
}
