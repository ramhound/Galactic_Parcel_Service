using UnityEngine;
using System.Collections;

public class HubUiManager : MonoBehaviour {
    public HubStation selectedStation;

    public void CreateShip() {
        var packet = new CommandPacket() {
            senderId = GamePlayer.localInstance.name,
            uuids = new string[] { selectedStation.name },
            command = GameCommand.Spawn,
            commandData = new Vector3((int)(ShipType.Cargo), 0, 0)
        };
        GamePlayer.localInstance.SendCommandPacket(packet);
    }

    public void CreatePackage() {
        selectedStation.GeneratePackages();
    }
}
