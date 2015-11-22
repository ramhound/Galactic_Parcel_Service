using UnityEngine;
using UnityEngine.Networking;
using Pathfinding;
using System.Collections;
using System;

public class ShipController : GameCommandHandler {
    private Seeker seeker;
    private Path path;
    private Rigidbody2D body2D;
    private Vector2 targetDestination;
    private int nodeIndex = 0;

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
        HandleCommand(currentCommand);
    }

    public override void HandleCommand(CommandPacket packet) {
        Debug.Log((GameCommand)packet.command);
        if(packet.command == (int)GameCommand.None) {
            return;
        } else if(packet.command == (int)GameCommand.Move) {
            SetDestination(packet.commandData);
        } else if(packet.command == (int)GameCommand.PickUp) {
            SetDestination(packet.commandData);
        }
    }

    private void FixedUpdate() {
        //i will have to eventually have to rewrite this to make a bit more sense
        if(!NetworkClient.active || isServer) {
            if(path != null) {
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
        targetDestination = destination;
        seeker.StartPath(transform.position, targetDestination, OnPathComplete);
    }

    //make this more generic and add it to pixel math
    private Quaternion RotateTowards(Transform trans, Vector2 point) {
        float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        return Quaternion.Slerp(trans.rotation, q, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(currentCommand.command == (int)GameCommand.PickUp &&
            col.name.EqualsIgnoreCase(currentCommand.senderId)) {

            currentCommand = new CommandPacket() { command = (int)GameCommand.None};
        }
    }
}
