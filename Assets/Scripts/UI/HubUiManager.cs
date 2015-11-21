using UnityEngine;
using System.Collections;

public class HubUiManager : MonoBehaviour {
    public HubStation selectedStation;

    public void CreateShip() {
        var packet = new CommandPacket() {
            playerId = GamePlayer.localInstance.name,
            uuids = new string[] { selectedStation.name },
            command = (int)PlayerCommand.Spawn,
            commandData = new Vector3((int)(Ship.ShipType.Cargo), 0, 0)
        };
        GamePlayer.localInstance.SendCommandPacket(packet);
    }
}
