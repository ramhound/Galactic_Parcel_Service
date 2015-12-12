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
    public GameObject hubMenu;
    public GameObject shipUI;
    public GameObject[] spawnables;
    public Transform spawnLocation;

    public List<Ship> activeFleet = new List<Ship>();
    public List<Ship> cargoPickUp = new List<Ship>();

    //packages Inherited from Location 
    public List<Package> shuttlePackages = new List<Package>();
    public List<Location> deliveryLocations = new List<Location>();
    public static List<Location> allHubStations = new List<Location>();
    public int claimedPackages = 0;
    public int PackagesAvail {
        get { return packages.Count - claimedPackages; }
    }

    public int claimedShuttlePackages = 0;
    public int ShuttlePackagesAvail {
        get { return shuttlePackages.Count - claimedShuttlePackages; }
    }

    public SyncRouteList cargoRoutes = new SyncRouteList();
    public SyncRouteList shuttleRoutes = new SyncRouteList();
    public SyncRouteList explorerRoutes = new SyncRouteList();

    public override void Start() {
        base.Start();
        if(Location.discoveredLocations.Contains(this)) //lazy so for now just remove ourselves
            Location.discoveredLocations.Remove(this);
        if(!HubStation.allHubStations.Contains(this))
            HubStation.allHubStations.Add(this);
        locationName = name;
        cargoPickUp.Clear();
    }

    public override void SetSelected(bool selected) {
        base.SetSelected(selected);
        //later ill do something to let others on the network know

        hubMenu.gameObject.SetActive(selected);
        hubMenu.GetComponent<HubUIManager>().selectedStation = selected ? this : null;
    }

    public override void OnGameTick() {
        //packageneration here
        base.OnGameTick();

        if(GameTimer.currentTick == 1) { //treating this tick like a late start
            //putting this here for now so i can populate the list of locations
            Setup();
        }

        if(GameTimer.currentTick % 50 == 0) { //treating this tick like a late start
            //putting this here for now so i can populate the list of locations
            GeneratePackages();
        }

        //broadcast pickup request to nearby ships not in route 
        if(PackagesAvail > 0) {
            BroadcastPickup(ShipType.Cargo);
        }

        if(ShuttlePackagesAvail > 0) {
            BroadcastPickup(ShipType.Shuttle);
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
            locations = Location.ToVectorArray(HubStation.allHubStations)
        });
    }

    private void BroadcastPickup(ShipType type) {
        //activeFleet.Sort(delegate(Ship x, Ship y) {
        //    var xDist = Vector2.Distance(x.transform.position, transform.position);
        //    var yDist = Vector2.Distance(y.transform.position, transform.position);
        //    Debug.Log(x.name);
        //    Debug.Log(y.name);
        //    return 1;
        //});

        foreach(var s in activeFleet) {
            if(s.atHub && s.type == type
                && (s.currentCommand == GameCommand.None || s.currentCommand == GameCommand.Return)) {
                //i need how many packages this ship is able to pick up and check it to what 
                //the hub has available so i can keep the claimed packeges count accurate
                int loadCount = s.cargoSize - s.CargoCount;
                if(s.type == ShipType.Cargo) {
                    claimedPackages += loadCount > PackagesAvail ? PackagesAvail : loadCount;
                } else {
                    claimedShuttlePackages += loadCount > ShuttlePackagesAvail 
                        ? ShuttlePackagesAvail : loadCount;
                }

                cargoPickUp.Add(s);
                s.ReceiveCommand(new CommandPacket() {
                    command = GameCommand.PickUp,
                    senderId = name,
                    commandData = transform.position
                });

                return;
            }
        }

        foreach(var s in activeFleet) {
            if(s.type == type
                && (s.currentCommand == GameCommand.None || s.currentCommand == GameCommand.Return)) {

                int loadCount = s.cargoSize - s.CargoCount;
                if(s.type == ShipType.Cargo) {
                    claimedPackages += loadCount > PackagesAvail ? PackagesAvail : loadCount;
                } else {
                    claimedShuttlePackages += loadCount > ShuttlePackagesAvail
                        ? ShuttlePackagesAvail : loadCount;
                }

                cargoPickUp.Add(s);
                s.ReceiveCommand(new CommandPacket() {
                    command = GameCommand.PickUp,
                    senderId = name,
                    commandData = transform.position
                });

                return;
            }
        }
    }

    public override void ExecuteCommand(GameCommand command) {
        base.ExecuteCommand(command);

        if(currentCommand == GameCommand.Spawn) {
            var shipGo = Instantiate(spawnables[(int)(commandData.x)]
                , spawnLocation.position, spawnables[(int)(commandData.x)]
                .transform.rotation) as GameObject;
            var ship = shipGo.GetComponent<Ship>();
            ship.hubStation = this;
            ship.shipUI = shipUI;
            activeFleet.Add(ship);
            if(isServer) NetworkServer.Spawn(shipGo);

            if(ship.type == ShipType.Cargo)
                ship.routes = cargoRoutes;
            else if(ship.type == ShipType.Shuttle) {
                ship.routes = shuttleRoutes;
            } else ship.routes = explorerRoutes;

            GamePlayer.localInstance.DisplayBanner(new Vector2(-1, 0), ship.type.ToString(), Banner.BannerType.Message);
            CompletedCommand(command);
        }
    }

    public override void DockWith(Ship ship) {
        if(ship.currentCommand == GameCommand.PickUp) {
            LoadPackages(ship);
        } else if(ship.currentCommand == GameCommand.Shuttle) {
            ShuttleDelivery(ship);
        }
    }

    public override void LoadPackages(Ship ship) {
        //iterate over all keys in cargo dic totaling up the packages
        int cargoIndex = ship.CargoCount;

        List<Package> loadList = new List<Package>(ship.cargoSize);
        if(ship.type == ShipType.Cargo) {
            for(int i = packages.Count - 1; i >= 0; i--) {
                var locList = ship.routes[0].locations.ToList();
                if(locList.Contains(packages[i].receiver.location.position)
                        && cargoIndex < ship.cargoSize) {

                    cargoIndex++;
                    loadList.Add(packages[i]);
                    packages.RemoveAt(i);
                }
            }

            claimedPackages -= loadList.Count;

            foreach(var p in loadList) {
                if(ship.cargo.ContainsKey(p.receiver.location))
                    ship.cargo[p.receiver.location].Add(p);
                else {
                    var tp = new List<Package>();
                    tp.Add(p);
                    ship.cargo[p.receiver.location] = tp;
                }
            }
        }

        if(ship.type == ShipType.Shuttle) {
            for(int i = shuttlePackages.Count - 1; i >= 0; i--) {
                var locList = ship.routes[0].locations.ToList();
                foreach(var sf in shuttlePackages[i].receiver.location.shipingFacilities) {
                    if(locList.Contains(sf.position) && cargoIndex < ship.cargoSize) {
                        cargoIndex++;
                        loadList.Add(shuttlePackages[i]);
                        shuttlePackages.RemoveAt(i);
                    }
                }
            }

            claimedShuttlePackages -= loadList.Count;

            foreach(var p in loadList) {
                if(ship.cargo.ContainsKey(p.receiver.location.shipingFacilities[0]))
                    ship.cargo[p.receiver.location.shipingFacilities[0]].Add(p);
                else {
                    var tp = new List<Package>();
                    tp.Add(p);
                    ship.cargo[p.receiver.location.shipingFacilities[0]] = tp;
                }
            }
        }
    }

    public void ShuttleDelivery(Ship ship) {
        //this should load all packages from the shuttle ship to the hub station
        var tl = new List<Location>();
        foreach(var kv in ship.cargo) {
            if(kv.Key == this) {
                foreach(var p in kv.Value) {
                    packages.Add(p);
                }
                tl.Add(kv.Key);
            }
        }

        foreach(var l in tl) {
            ship.cargo.Remove(l);
        }


        //for(int i = ship.cargo.Count - 1; i >= 0; i--) {
        //    if(deliveryLocations.Contains(ship.cargo[i].receiver.location)) {
        //        packages.Add(ship.cargo[i]);
        //        ship.cargo.RemoveAt(i);
        //    }
        //}

        //need to rework the broadcas fro shuttle and issue #6 will finally be fixed
        //but for now it is working fine as long as i dont have the shuttle pick up packages from the hub
        //it just delivered to

        //if(shuttlePackages.Count > 0)
        //    LoadPackages(ship);
        UpdateAvailablePackages();
    }

    public void GeneratePackages() {
        if(deliveryLocations.Count == 0) return; //let this through with different vars
        int packageCount = Random.Range(1, 3);

        for(int i = 0; i < packageCount; i++) {
            var package = new Package() {
                sender = ClientManager.GenerateClient(
                    Location.discoveredLocations[Random.Range(0, Location.discoveredLocations.Count)]),
                receiver = ClientManager.GenerateClient(
                    Location.discoveredLocations[Random.Range(0, Location.discoveredLocations.Count)]),
                fragility = 1f,
                size = Vector2.one
            };

            if(deliveryLocations.Contains(package.receiver.location)) {
                packages.Add(package);
            } else {
                shuttlePackages.Add(package);
            }
        }

        UpdateAvailablePackages();
        //sort the package list when there is a sorting facility on this hub
    }

    public void UpdateAvailablePackages() {
        //prob a better way buty im despriote
        claimedPackages = 0;
        claimedShuttlePackages = 0;
        foreach(var ship in cargoPickUp) {
            int loadCount = ship.cargoSize - ship.CargoCount;
            if(ship.type == ShipType.Cargo) {
                claimedPackages += loadCount > PackagesAvail ? PackagesAvail : loadCount;
            } else {
                claimedShuttlePackages += loadCount > ShuttlePackagesAvail
                    ? ShuttlePackagesAvail : loadCount;
            }
        }
    }

    public override void OnTriggerEnter2D(Collider2D col) { /*left blank for the override*/ }
}
