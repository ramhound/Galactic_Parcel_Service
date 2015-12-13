using UnityEngine;
using UnityEngine.Networking;
using Pathfinding;
using System.Collections.Generic;
using System;

public class ShipController : NetworkBehaviour {
    private Seeker seeker;
    private Path path;
    private Ship ship;
    private int nodeIndex = 0;
    public Rigidbody2D body2D;

    public float speed = 100f;
    public float rotationSpeed = 30f;
    public float endPointDistance = .3f;
    public float pathRefreshRate = 0.1f;

    public void Start() {
        seeker = GetComponent<Seeker>();
        body2D = GetComponent<Rigidbody2D>();
        ship = GetComponent<Ship>();
    }

    private void OnPathComplete(Path path) {
        if(!path.error) {
            this.path = path;
            nodeIndex = 0;
        }
    }

    private void FixedUpdate() {
        //i will have to eventually have to rewrite this to make a bit more sense
        if(!NetworkClient.active || isServer) {
            if(path != null && ship.currentCommand != GameCommand.None) {
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

    public void SetDestination(Location loc) {
        SetDestination(loc.position);
    }

    public void SetDestination(Vector2 destination) {
        seeker.StartPath(transform.position, destination, OnPathComplete);
        var dist = Vector2.Distance(transform.position, ship.dataVector);
    }

    public void Stop() {
        body2D.velocity = Vector2.zero;
        path = null;
    }

    //make this more generic and add it to pixel math
    private Quaternion RotateTowards(Transform trans, Vector2 point) {
        float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        return Quaternion.Slerp(trans.rotation, q, Time.deltaTime * rotationSpeed);
    }
}
