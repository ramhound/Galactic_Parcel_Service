using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipUIManager : MonoBehaviour {
    public Ship selectedShip;
    public Text currentCmdLabel;
    public Text cargoLabel;
    public GameObject distMarkerPrefab;

    private void Update() {
        currentCmdLabel.text = "Command: " + selectedShip.currentCommand.ToString();
        cargoLabel.text = "Cargo Count: " + selectedShip.cargo.Count;
    }
}
