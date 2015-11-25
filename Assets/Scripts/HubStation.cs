using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public struct Route {
    public Vector2[] locations;
    public string name;
    public bool timeSort;
    public bool distanceSort;
}
public class HubStation : Location {
    public HubUiManager hubUiManager;
    public GameObject hubMenu;
    public GameObject[] spawnables;
    public List<Ship> activeFleet = new List<Ship>();
    public List<Location> deliveryLocations = new List<Location>();
    public SyncRouteList routes = new SyncRouteList();
    public static Route defaultRoute;

    public override void Start() {
        base.Start();
        locationName = "Hub Station";
    }

    public override void SetSelected(bool selected) {
        base.SetSelected(selected);
        //later ill do something to let others on the network know

        hubMenu.gameObject.SetActive(selected);
        hubUiManager.selectedStation = selected ? this : null;
    }

    public override void OnGameTick() {
        //packageneration here
        base.OnGameTick();

        if(GameTimer.currentTick == 5) {
            //putting this here for now so i can populate the list of locations
            Setup();
        }

        //broadcast pickup request to nearby ships not in route 
        if(packages.Count > 0) {
            BroadcastPickup();
        }
    }

    private void Setup() {
        defaultRoute = new Route() {
            name = "Default",
            locations = Location.ToVectorArray(deliveryLocations),
            timeSort = false,
            distanceSort = false
        };
        GeneratePackages();
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
            var shipGo = Instantiate(spawnables[(int)(commandData.x)]) as GameObject;
            var ship = shipGo.GetComponent<Ship>();
            ship.hubStation = this;
            activeFleet.Add(ship);
            if(isServer) NetworkServer.Spawn(shipGo);

            ship.routes.Add(defaultRoute);
            CompletedCommand(command);
        }
    }

    public override void ReceiveCommand(CommandPacket packet) {
        base.ReceiveCommand(packet);
    }

    public void GeneratePackages() {
        if(deliveryLocations.Count == 0) return; //let this through with different vars

        int packageCount = Random.Range(1, 3);
        for(int i = 0; i < packageCount; i++) {
            var package = new Package() {
                sender = ClientManager.GenerateClient(deliveryLocations[Random.Range(0, deliveryLocations.Count)]),
                receiver = ClientManager.GenerateClient(deliveryLocations[Random.Range(0, deliveryLocations.Count)]),
                fragility = 1f,
                size = Vector2.one
            };
            packages.Add(package);
        }
    }
}
