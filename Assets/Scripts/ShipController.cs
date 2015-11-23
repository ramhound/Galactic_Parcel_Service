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

    public List<Package> packages = new List<Package>();
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
        if(currentCommand == GameCommand.None) {
            return;
        } else if(currentCommand == GameCommand.Move) {
            SetDestination(commandData);
        } else if(currentCommand == GameCommand.PickUp) {
            SetDestination(commandData);
        } else if(currentCommand == GameCommand.Deliver) {
            SetDestination(commandData);
        }
    }

    //public override void HandleCommand(CommandPacket packet) {
    //    base.HandleCommand(packet);
    //}

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

    //make this more generic and add it to pixel math
    private Quaternion RotateTowards(Transform trans, Vector2 point) {
        float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        return Quaternion.Slerp(trans.rotation, q, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(!NetworkClient.active || isServer) {
            if(currentCommand == GameCommand.PickUp && col.name == commandSenderId) {
                var loc = col.GetComponent<Location>();
                foreach(var p in loc.packages) {
                    packages.Add(p);
                }

                loc.packages.Clear();
                CompletedCommand(currentCommand);

                //set new command
                if(packages.Count > 0) {
                    Debug.Log(packages[0].receiver.location);
                    ReceiveCommand(new CommandPacket() {
                        command = GameCommand.Deliver,
                        commandData = packages[0].receiver.location.transform.position,
                        senderId = packages[0].receiver.location.name
                    });
                    Debug.Log("Heading out for delivery");
                }
            } else if(currentCommand == GameCommand.Deliver && col.name == commandSenderId) {
                body2D.velocity = Vector2.zero;
                CompletedCommand(currentCommand);

                Debug.Log(packages[0].receiver.profilePicIndex);
                GamePlayer.localInstance.DisplayBanner(packages[0].receiver.profilePicIndex, 
                    @"<size=40><b>Fuck yea man!</b></size> /n Thanks for making sure that it got here in one peice",
                    Banner.BannerType.Package);
            }
        }
    }
}
