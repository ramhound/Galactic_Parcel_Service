using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;

public enum ShipType { Cargo = 0, Shuttle, Speed, Defense, Special, Attack, Enemy_Attack, Enemy_Speed, Enemy_Defense }
public class Ship : GameCommandHandler, ISelectable {
    public ShipType type = ShipType.Cargo;
    public HubStation hubStation;
    public SyncRouteList routes = new SyncRouteList();
    public List<Package> cargo = new List<Package>(); //may need to sync
    public ShipController shipController;
    public Location dockedLocation;
    //public Vector2 cargoSpace = Vector2.one;

    public void SetSelected(bool selected) {
        if(selected) {

        }
    }

    public void OnClick() {
        GamePlayer.localInstance.SetSelectedUnits(transform);
    }

    public override void OnGameTick() {
        base.OnGameTick();
    }

    public override void ExecuteCommand(GameCommand command) {
        if(command == GameCommand.None) {
            if(cargo.Count > 0)
                StartDelivery();
        } else {
            shipController.SetDestination(commandData);
        }
    }

    public override void CompletedCommand(GameCommand command) {
        base.CompletedCommand(command);
        shipController.Stop();
    }

    private void StartDelivery() {
        if(type == ShipType.Cargo) {
            ReceiveCommand(new CommandPacket() {
                command = GameCommand.Deliver,
                commandData = cargo[0].receiver.location.transform.position,
                senderId = cargo[0].receiver.location.locationName
            });

            Debug.Log(name + " Heading out for delivery");
        } else if(type == ShipType.Shuttle) {
            ReceiveCommand(new CommandPacket() {
                command = GameCommand.Shuttle,
                commandData = cargo[0].shippingFacility.position,
                senderId = dockedLocation.name
            });
        }
    }

    private void DockWith(Location loc) {
        //anim for dock
        dockedLocation = loc;
        loc.DockWith(this);
    }

    public void LoadPackages(List<Package> packages) {
        if(type == ShipType.Cargo) {
            for(int i = packages.Count - 1; i >= 0; i--) {
                //i think in the future i am going to not use a list and just check location
                var locList = routes[0].locations.ToList();
                if(locList.Contains(packages[i].receiver.location.position)) {
                    cargo.Add(packages[i]);
                    packages.RemoveAt(i);
                }
            }
        } else if(type == ShipType.Shuttle) {
            for(int i = packages.Count - 1; i >= 0; i--) {
                Debug.Log(packages[i].receiver.location);
                var locList = routes[0].locations.ToList();
                if(locList.Contains(packages[i].shippingFacility.position)) {
                    cargo.Add(packages[i]);
                    packages.RemoveAt(i);
                }
            }
        }

        //cool tetris logic here...later
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(!NetworkClient.active || isServer) {
            if(col.tag == "Hub Station" || col.tag == "Planet") {
                var loc = col.GetComponent<Location>();
                if(commandSenderId == loc.locationName) {
                    if(currentCommand == GameCommand.PickUp) {
                        DockWith(loc);
                        CompletedCommand(currentCommand);

                        if(cargo.Count > 0)
                            StartDelivery();

                    } else if(currentCommand == GameCommand.Deliver
                        || currentCommand == GameCommand.Shuttle) {

                        for(int i = cargo.Count - 1; i >= 0; i--) {
                            if(cargo[i].receiver.location == loc) {
                                cargo[i].receiver.PackageDelivered();
                                cargo.RemoveAt(i);

                                //                            GamePlayer.localInstance.DisplayBanner(cargo[0].receiver.profilePicIndex,
                                //@"<size=32>Fuck yea man!</size> \\nThanks for making sure that it got here in one peice",
                                //Banner.BannerType.Package);
                            }
                        }

                        CompletedCommand(currentCommand);
                    }
                }
            }
        }
    }
}
public class ShipStats {
    public int health;
    public int defense;
    public int speed;
    public int cargoSpace;
}
