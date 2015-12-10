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
    public int cargoSize = 3; //will rework this later
    public bool atHub = false;

    public void SetSelected(bool selected) { }

    public void OnClick() {
        GamePlayer.localInstance.SetSelectedUnits(transform);
    }

    public override void ExecuteCommand(GameCommand command) {
        if(command == GameCommand.None) {
            Debug.Log(currentCommand);
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
            ReceiveCommand(new CommandPacket() {
                command = GameCommand.Delivery,
                commandData = cargo[0].receiver.location.transform.position,
                senderId = cargo[0].receiver.location.locationName
            });

            Debug.Log(name + " Heading out to " + cargo[0].receiver.location + " for delivery");
        } else if(type == ShipType.Shuttle) {
            //foreach(var sf in cargo[0].receiver.location.shipingFacilities) {
            //    if(sf)
            //}

            ReceiveCommand(new CommandPacket() {
                command = GameCommand.Shuttle,
                //i think this might be where it is getting 
                commandData = cargo[0].receiver.location.shipingFacilities[0].position,
                senderId = cargo[0].receiver.location.shipingFacilities[0].name
            });
            Debug.Log(name + " Heading out " + cargo[0].receiver.location.shipingFacilities[0] + " for shuttle");
        }
    }

    private void DockWith(Location loc) {
        //anim for dock
        shipController.Stop();
        dockedLocation = loc;
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
