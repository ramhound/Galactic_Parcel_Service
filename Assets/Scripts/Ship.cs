﻿using UnityEngine;
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
        ReceiveCommand(new CommandPacket() {
            command = GameCommand.Deliver,
            commandData = cargo[0].receiver.location.transform.position,
            senderId = cargo[0].receiver.location.locationName
        });

        Debug.Log(name + " Heading out for delivery");
    }

    private void DockWith(Location loc) {
        //anim for dock
        loc.DockWith(this);
    }

    public void LoadPackages(List<Package> packages) {
        //cool tetris logic here...later
        for(int i = packages.Count - 1; i >= 0; i--) {
            var locList = routes[0].locations.ToList();
            if(locList.Contains(packages[i].receiver.location.position)) {
                cargo.Add(packages[i]);
                packages.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(!NetworkClient.active || isServer) {
            if(col.tag == "Hub Station" || col.tag == "Planet") {
                var loc = col.GetComponent<Location>();
                if(commandSenderId == loc.locationName) {
                    if(currentCommand == GameCommand.PickUp) {
                        DockWith(loc);
                        CompletedCommand(currentCommand);
                        hubStation.GeneratePackages();

                        if(cargo.Count > 0)
                            StartDelivery();

                    } else if(currentCommand == GameCommand.Deliver) {
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