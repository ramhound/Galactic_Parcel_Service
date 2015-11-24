using UnityEngine;
using UnityEngine.Networking;
using Pathfinding;
using System.Collections.Generic;
using System;

public class ShipController : GameCommandHandler {
    private Seeker seeker;
    private Path path;
    private Rigidbody2D body2D;
    private int nodeIndex = 0;

    public Vector2 cargoSpace = Vector2.one;
    //public Package[,] cargo;
    public List<Package> cargo;
    public float speed = 100f;
    public float rotationSpeed = 30f;
    public float endPointDistance = .3f;
    public float pathRefreshRate = 0.1f;
    private static int idIndex = 0;

    public override void Start() {
        base.Start();

        seeker = GetComponent<Seeker>();
        body2D = GetComponent<Rigidbody2D>();
        name = "Ship " + idIndex++;

        //setup ship based on the hub that hired it
        //cargo = new Package[(int)cargoSpace.x, (int)cargoSpace.y];
        cargo = new List<Package>();
    }

    private void OnPathComplete(Path path) {
        if(!path.error) {
            this.path = path;
            nodeIndex = 0;
        }
    }

    public override void OnGameTick() {
        base.OnGameTick();
    }

    public override void ExecuteCommand(GameCommand command) {
        if(command == GameCommand.None) {
            if(cargo.Count > 0)
                StartDelivery();
        } else if(command == GameCommand.Move) {
            SetDestination(commandData);
        } else if(command == GameCommand.PickUp) {
            SetDestination(commandData);
        } else if(command == GameCommand.Deliver) {
            SetDestination(commandData);
        }
    }

    private void FixedUpdate() {
        //i will have to eventually have to rewrite this to make a bit more sense
        if(!NetworkClient.active || isServer) {
            if(path != null && currentCommand != GameCommand.None) {
                //completed trip
                if(nodeIndex >= path.vectorPath.Count) {
                    body2D.velocity = Vector2.zero;
                    return;
                }

                var dir = (path.vectorPath[nodeIndex] - transform.position).normalized;
                dir *= speed * Time.fixedDeltaTime;
                body2D.velocity = dir;
                transform.rotation = RotateTowards(transform, dir);

                if(Vector2.Distance(transform.position, path.vectorPath[nodeIndex]) < endPointDistance) {
                    nodeIndex++;
                    return;
                }
            }
        }
    }

    public void SetDestination(Vector2 destination) {
        seeker.StartPath(transform.position, destination, OnPathComplete);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(!NetworkClient.active || isServer) {
            if(col.tag == "Hub Station" || col.tag == "Planet") {
                var loc = col.GetComponent<Location>();
                if(commandSenderId == loc.locationName) {


                    if(currentCommand == GameCommand.PickUp) {
                        LoadShip(loc.packages);
                        CompletedCommand(currentCommand);

                    } else if(currentCommand == GameCommand.Deliver) {
                        body2D.velocity = Vector2.zero;
                        GamePlayer.localInstance.DisplayBanner(cargo[0].receiver.profilePicIndex,
                            @"<size=32>Fuck yea man!</size> /\n Thanks for making sure that it got here in one peice",
                            Banner.BannerType.Package);
                        cargo.Remove(cargo[0]);
                        CompletedCommand(currentCommand);
                    }
                }
            }
        }
    }

    private void LoadShip(List<Package> packages) {
        //cool tetris logic here...later
        for(int i = packages.Count - 1; i >= 0; i--) {
            cargo.Add(packages[i]);
            packages.Remove(packages[i]);
        }

        //i dont like this i will change later
        //var cargoIndex = new Vector2(-1, -1);

        //for(int i = packages.Count - 0; i > 0; i--) {
        //    var tempCargoIndex = cargoIndex + packages[i].size;
        //    if(tempCargoIndex.x < cargoSpace.x && tempCargoIndex.y < cargoSpace.y) {
        //        cargoSpace += packages[i].size;
        //        cargo[(int)cargoSpace.x, (int)cargoSpace.y] = packages[i];
        //    } else { Debug.Log("Package didnt fit"); }
        //}

        //packages.Clear();
    }

    private void StartDelivery() {
        ReceiveCommand(new CommandPacket() {
            command = GameCommand.Deliver,
            commandData = cargo[0].receiver.location.transform.position,
            senderId = cargo[0].receiver.location.locationName
        });

        Debug.Log("Heading out for delivery");
    }

    //make this more generic and add it to pixel math
    private Quaternion RotateTowards(Transform trans, Vector2 point) {
        float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        return Quaternion.Slerp(trans.rotation, q, Time.deltaTime * rotationSpeed);
    }
}
