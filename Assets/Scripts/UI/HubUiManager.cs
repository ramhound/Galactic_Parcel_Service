using UnityEngine;
using System.Collections;

public class HubUiManager : MonoBehaviour {
    public HubStation selectedStation;

    private void NotifyGamePlayer(CommandPacket packet) {
        GamePlayer.localInstance.SendCommandPacket(packet);
    }

    public void CreateShip() {
        var packet = new CommandPacket() {
            playerId = GamePlayer.localInstance.name,
            uuids = new string[] { selectedStation.name },
            command = (int)PlayerCommand.Spawn,
            commandData = "Cargo Ship"
        };
        NotifyGamePlayer(packet);
    }
}
