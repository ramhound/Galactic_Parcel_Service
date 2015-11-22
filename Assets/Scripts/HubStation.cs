using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HubStation : Location {
    public HubUiManager hubUiManager;
    public GameObject hubMenu;
    public GameObject[] spawnables;
    public List<ShipController> activeFleet = new List<ShipController>();

    public override void Start() {
        base.Start();
    }

    public override void OnClick() {
        GamePlayer.localInstance.uuids = new string[] { };

        SetSelected(true);
    }

    public override void SetSelected(bool selected) {
        base.SetSelected(selected);

        hubMenu.gameObject.SetActive(selected);
        hubUiManager.selectedStation = selected ? this : null;
    }

    public override void OnGameTick() {
        //packageneration here

        if(GameTimer.currentTick == 1) {
            packages.Add(new Package() {
                fragility = 1,
                sender = ClientManager.farnsberg,
                receiver = ClientManager.GenerateClient(Location.GetNearestLocation(transform.position)),
                size = Vector2.one
            });
        }

        //broadcast pickup request to nearby ships not in route 
        if(packages.Count > 0) {
            foreach(var s in activeFleet) {
                if(s.currentCommand.command == (int)GameCommand.None) {
                    s.HandleCommand(new CommandPacket() {
                        command = (int)GameCommand.PickUp,
                        senderId = name,
                        commandData = transform.position
                    });
                }
            }
        }
    }

    public override void HandleCommand(CommandPacket packet) {
        base.HandleCommand(packet);

        if(packet.command == (int)GameCommand.Spawn) {
            var ship = Instantiate(spawnables[(int)(packet.commandData.x)]) as GameObject;
            activeFleet.Add(ship.GetComponent<ShipController>());            

            if(isServer) NetworkServer.Spawn(ship);
        }
    }

    private Package CreatePackage() {
        var package = new Package() {
            sender = ClientManager.GenerateClient(Location.GetNearestLocation(transform.position)),
            receiver = ClientManager.GenerateClient(Location.GetNearestLocation(transform.position)),
            fragility = 1f,
            size = Vector2.one
        };

        PopUp.DisplayBanner(package.receiver.profilePic, "Reveiver", Banner.BannerType.Message);
        return package;
    }

    private void OnTriggerEnter2D(Collider2D col) {

    }
}
