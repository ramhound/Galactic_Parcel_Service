using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HubStation : Location {
    public HubUiManager hubUiManager;
    public GameObject hubMenu;
    public GameObject[] spawnables;
    public List<ShipController> activeFleet = new List<ShipController>();
    public List<Location> deliveryLocations = new List<Location>();

    public override void Start() {
        base.Start();

        locationName = "Hub Station";
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
        base.OnGameTick();

        if(GameTimer.currentTick == 5) {
            //packages.Add(new Package() { //convert this into a command
            //    fragility = 1,
            //    sender = ClientManager.farnsberg,
            //    receiver = ClientManager.GenerateClient(Location.GetNearestLocation(transform.position)),
            //    size = Vector2.one
            //});
            GeneratePackages();
        }

        //broadcast pickup request to nearby ships not in route 
        Debug.Log(packages.Count);
        if(packages.Count > 0) {
            BroadcastPickup();
        }
    }

    private void BroadcastPickup() {
        foreach(var s in activeFleet) {
            if(s.currentCommand == GameCommand.None) {
                s.ReceiveCommand(new CommandPacket() {
                    command = GameCommand.PickUp,
                    senderId = name,
                    commandData = transform.position
                });
            }
        }
    }

    public override void ExecuteCommand(GameCommand command) {
        base.ExecuteCommand(command);

        if(currentCommand == GameCommand.Spawn) {
            var ship = Instantiate(spawnables[(int)(commandData.x)]) as GameObject;
            activeFleet.Add(ship.GetComponent<ShipController>());
            if(isServer) NetworkServer.Spawn(ship);

            CompletedCommand(command);
        }
    }

    public override void ReceiveCommand(CommandPacket packet) {
        base.ReceiveCommand(packet);
    }

    public void GeneratePackages() {
        if(deliveryLocations.Count == 0) return; //let this through with different vars

        int packageCount = UnityEngine.Random.Range(0, 5);

        for(int i = 0; i < packageCount; i++) {
            var package = new Package() {
                sender = ClientManager.GenerateClient(deliveryLocations[UnityEngine.Random.Range(0, deliveryLocations.Count - 1)]),
                receiver = ClientManager.GenerateClient(deliveryLocations[UnityEngine.Random.Range(0, deliveryLocations.Count - 1)]),
                fragility = 1f,
                size = Vector2.one
            };
            packages.Add(package);
            Debug.Log(packages.Count);
        }
    }
}
