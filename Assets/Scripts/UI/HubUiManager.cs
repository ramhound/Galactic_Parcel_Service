using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HubUIManager : MonoBehaviour {
    public HubStation selectedStation;
    public Text cargoPackages;
    public Text shuttlePackages;

    public void CreateShip(int shipType) {
        GamePlayer.localInstance.SendCommandPacket(new CommandPacket() {
            dataString = GamePlayer.localInstance.name,
            uuids = new string[] { selectedStation.name },
            command = GameCommand.Spawn,
            dataVector = new Vector3(shipType, 0, 0)
        });
    }

    public void CreatePackage() {
        selectedStation.GeneratePackages();
    }

    private void Update() {
        cargoPackages.text = "Cargo Packages: " + selectedStation.packages.Count;
        shuttlePackages.text = "Shuttle Packages: " + selectedStation.shuttlePackages.Count;
    }
}
