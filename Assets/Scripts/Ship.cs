using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public enum ShipType { Cargo = 0, Shuttle, Speed, Defense, Special, Attack, Enemy_Attack, Enemy_Speed, Enemy_Defense }
public class Ship : GameCommandHandler, ISelectable {
    public ShipType type = ShipType.Cargo;
    public HubStation hubStation;
    public SyncRouteList routes = new SyncRouteList();
    //may still want to sync the packages for ui purposes
    public Dictionary<Location, List<Package>> cargo = new Dictionary<Location, List<Package>>();
    public ShipController shipController;

    public GameObject shipUI;
    public int cargoSize = 3; //will rework this later
    public bool atHub = false;

    public void SetSelected(bool selected) {
        shipUI.SetActive(selected);
        shipUI.GetComponent<ShipUIManager>().selectedShip = selected ? this : null;
    }

    public void OnClick() {
        GamePlayer.localInstance.SetSelectedUnits(transform);
    }

    public override void ExecuteCommand(GameCommand command) {
        if(command == GameCommand.None) {
            if(cargo.Count > 0) {
                StartDelivery();
            } else if(!atHub) {
                ReceiveCommand(new CommandPacket {
                    command = GameCommand.Return,
                    commandData = hubStation.position,
                    senderId = hubStation.name
                });
            }
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
            var loc = cargo.Keys.First();
            ReceiveCommand(new CommandPacket() {
                command = GameCommand.Delivery,
                commandData = loc.position,
                senderId = loc.locationName
            });

            Debug.Log(name + " Heading out to " + loc.locationName + " for delivery");
        } else if(type == ShipType.Shuttle) {
            var loc = cargo.Keys.First();
            ReceiveCommand(new CommandPacket() {
                command = GameCommand.Shuttle,
                //i think this might be where it is getting 
                commandData = loc.position,
                senderId = loc.name
            });
            Debug.Log(name + " Heading out " + loc.locationName + " for shuttle");
        }
    }

    private void DockWith(Location loc) {
        //anim for dock
        shipController.Stop();
        loc.DockWith(this);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(!NetworkClient.active || isServer) {
            if(col.tag == "Shield Collider") {
                var hs = col.GetComponentInParent<HubStation>();
                if(hs.name == hubStation.name) {
                    atHub = true;
                    if(currentCommand == GameCommand.PickUp && hs.name == commandSenderId) {
                        if(!hs.cargoPickUp.Contains(this))
                            CompletedCommand(GameCommand.Return);
                    } else if(currentCommand == GameCommand.Return && hs.name == commandSenderId) {
                        CompletedCommand(currentCommand);
                    }
                }

            } else if(col.tag == "Hub Station" || col.tag == "Location") {
                var loc = col.GetComponent<Location>();
                if(currentCommand == GameCommand.PickUp && loc.name == commandSenderId) {
                    DockWith(loc);
                    CompletedCommand(currentCommand);

                    var hs = (HubStation)loc;
                    hs.cargoPickUp.Remove(this);

                } else if((currentCommand == GameCommand.Delivery || currentCommand == GameCommand.Shuttle)
                        && commandSenderId == loc.name) {
                    DockWith(loc);
                    CompletedCommand(currentCommand);
                }

                if(cargo.Count > 0)
                    StartDelivery();
            }
        }
    }

    //public void AssignRoute(Route route) {
    //    routes.Add(route);
    //}

    private void OnTriggerExit2D(Collider2D col) {
        if(!NetworkClient.active || isServer) {
            if(col.tag == "Shield Collider") {
                var hs = col.GetComponentInParent<HubStation>();
                if(hs == hubStation)
                    atHub = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D col) { }
}
public struct ShipStats {
    public int health;
    public int defense;
    public int speed;
    public int cargoSpace;
}
