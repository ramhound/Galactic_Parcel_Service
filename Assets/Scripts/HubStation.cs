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
    public List<Package> shuttlePackages = new List<Package>();
    public List<Location> deliveryLocations = new List<Location>();
    public static List<Location> allHubStations = new List<Location>();

    public SyncRouteList cargoRoutes = new SyncRouteList();
    public SyncRouteList shuttleRoutes = new SyncRouteList();
    public SyncRouteList explorerRoutes = new SyncRouteList();

    public override void Start() {
        base.Start();
        if(discoveredLocations.Contains(this)) //lazy so for now just remove ourselves
            discoveredLocations.Remove(this);
        if(!allHubStations.Contains(this))
            allHubStations.Add(this);
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

        ////every 5 seconds add packages
        //if(GameTimer.currentTick % 25 == 0)
        //    GeneratePackages();

        //broadcast pickup request to nearby ships not in route 
        if(packages.Count > 0) {
            BroadcastPickup();
        }

        if(shuttlePackages.Count > 0) {
            BroadcastShuttlePickup();
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
            if(s.type == ShipType.Cargo && s.currentCommand == GameCommand.None) {
                s.ReceiveCommand(new CommandPacket() {
                    command = GameCommand.PickUp,
                    senderId = name,
                    commandData = transform.position
                });
            }
        }
    }

    private void BroadcastShuttlePickup() {
        foreach(var s in activeFleet) {
            if(s.type == ShipType.Shuttle && s.currentCommand == GameCommand.None) {
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

    public override void DockWith(Ship ship) {
        if(ship.currentCommand == GameCommand.PickUp || ship.currentCommand == GameCommand.Shuttle) {
            LoadPackages(ship);
        } else if(ship.currentCommand == GameCommand.Shuttle) {
            ShuttleDelivery(ship);
        }
    }

    public override void LoadPackages(Ship ship) {
        if(ship.type == ShipType.Cargo) {
            for(int i = packages.Count - 1; i >= 0; i--) {
                //i think in the future i am going to not use a list and just check location
                var locList = ship.routes[0].locations.ToList();
                if(locList.Contains(packages[i].receiver.location.position)) {
                    ship.cargo.Add(packages[i]);
                    packages.RemoveAt(i);
                }
            }
        } else if(ship.type == ShipType.Shuttle) {
            for(int i = shuttlePackages.Count - 1; i >= 0; i--) {
                //i think in the future i am going to not use a list and just check location
                var locList = ship.routes[0].locations.ToList();
                foreach(var sf in shuttlePackages[i].receiver.location.shipingFacilities) {
                    if(locList.Contains(sf.position)) {
                        ship.cargo.Add(shuttlePackages[i]);
                        shuttlePackages.RemoveAt(i);
                    }
                }
            }
        }
    }

    public void ShuttleDelivery(Ship ship) {
        //this should load all packages from the shuttle ship to the hub station
        for(int i = ship.cargo.Count - 1; i >= 0; i--) {
            if(ship.cargo[i].receiver.location.shipingFacilities.Contains(this)) {
                packages.Add(ship.cargo[i]);
                ship.cargo.RemoveAt(i);
            }
        }

        //if(shuttlePackages.Count > 0)
        //    LoadPackages(ship);
    }

    public void GeneratePackages() {
        if(deliveryLocations.Count == 0) return; //let this through with different vars
        int packageCount = Random.Range(1, 2);


        for(int i = 0; i < packageCount; i++) {
            var package = new Package() {
                sender = ClientManager.GenerateClient(discoveredLocations[Random.Range(0, discoveredLocations.Count)]),
                receiver = ClientManager.GenerateClient(discoveredLocations[Random.Range(0, discoveredLocations.Count)]),
                fragility = 1f,
                size = Vector2.one
            };

            if(package.receiver.location.shipingFacilities.Contains(this)) {
                packages.Add(package);
            } else {
                shuttlePackages.Add(package);
            }
        }

        //sort the package list when there is a sorting facility on this hub
    }

    public override void OnTriggerEnter2D(Collider2D col) {
        //left blank for override
    }
}
