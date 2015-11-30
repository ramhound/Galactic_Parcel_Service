using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public enum RouteType { Cargo = 0, Shuttle, Explorer }
public struct Route {
    public Vector2[] locations;
    public RouteType type;
    public string name;
    public bool timeSort;
    public bool distanceSort;
}
public class HubStation : Location {
    public HubUiManager hubUiManager;
    public GameObject hubMenu;
    public GameObject[] spawnables;
    public Transform spawnLocation;

    public List<Ship> activeFleet = new List<Ship>();
    public List<Location> deliveryLocations = new List<Location>();
    public static List<Location> allHubStations = new List<Location>();

    public SyncRouteList cargoRoutes = new SyncRouteList();
    public SyncRouteList shuttleRoutes = new SyncRouteList();
    public SyncRouteList explorerRoutes = new SyncRouteList();

    public override void Start() {
        base.Start();
        if(discoveredLocations.Contains(this)) //lazy so for now just remove ourselves
            discoveredLocations.Remove(this);
        locationName = name;
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

        if(GameTimer.currentTick == 1) { //treating this tick like a late start
            //putting this here for now so i can populate the list of locations
            Setup();
        }

        //broadcast pickup request to nearby ships not in route 
        if(packages.Count > 0 && GameTimer.currentTick % 2 == 0) {
            BroadcastPickup();
        }
    }

    private void Setup() {
        cargoRoutes.Add(new Route() {
            name = "Default",
            type = RouteType.Cargo,
            locations = Location.ToVectorArray(deliveryLocations),
            timeSort = false,
            distanceSort = false
        });

        shuttleRoutes.Add(new Route() {
            name = "Default",
            type = RouteType.Shuttle,
            locations = Location.ToVectorArray(allHubStations)
        });
        //shuttleRoutes
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
            var shipGo = Instantiate(spawnables[(int)(commandData.x)], 
                                     spawnLocation.position, Quaternion.identity) as GameObject;
            var ship = shipGo.GetComponent<Ship>();
            ship.hubStation = this;
            activeFleet.Add(ship);
            if(isServer) NetworkServer.Spawn(shipGo);

            if(ship.type == ShipType.Cargo)
                ship.routes = cargoRoutes;
            else if(ship.type == ShipType.Shuttle)
                ship.routes = shuttleRoutes;
            else ship.routes = explorerRoutes;

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
                sender = ClientManager.GenerateClient(discoveredLocations[Random.Range(0, discoveredLocations.Count)]),
                receiver = ClientManager.GenerateClient(discoveredLocations[Random.Range(0, discoveredLocations.Count)]),
                fragility = 1f,
                size = Vector2.one
            };
            packages.Add(package);
        }
    }
}
