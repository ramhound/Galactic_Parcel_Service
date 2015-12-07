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
    public bool atHub = false;

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
            else if(!atHub){
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

            Debug.Log(name + " Heading out for delivery");
        } else if(type == ShipType.Shuttle) {
            ReceiveCommand(new CommandPacket() {
                command = GameCommand.Shuttle,
                commandData = cargo[0].receiver.location.shipingFacilities[0].position,
                senderId = cargo[0].receiver.location.shipingFacilities[0].name
            });
            Debug.Log(name + " Heading out for shuttle");
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
            if(col.tag == "Hub Station" || col.tag == "Location") {
                var loc = col.GetComponent<Location>();
                if(commandSenderId == loc.locationName) {
                    DockWith(loc);
                    CompletedCommand(currentCommand);
                }
            } else if(col.tag == "Shield Collider") {
                var loc = col.GetComponentInParent<Location>();
                if(commandSenderId == loc.locationName) {
                    shipController.Stop();
                    atHub = true;
                    CompletedCommand(currentCommand);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col) {

    }

    private void OnTriggerExit2D(Collider2D col) {
        if(!NetworkClient.active || isServer) {
            if(col.tag == "Hub Station" || col.tag == "Location") {
                //var loc = col.GetComponent<Location>();
                //loc.UnDock(this);
                dockedLocation = null;
            } else if(col.tag == "Shield Collider") {
                atHub = false;
            }
        }
    }
}
public struct ShipStats {
    public int health;
    public int defense;
    public int speed;
    public int cargoSpace;
}
