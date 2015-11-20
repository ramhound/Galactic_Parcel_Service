using UnityEngine;
using UnityEngine.Networking;
using Pathfinding;
using System.Collections;
using System;

public class ShipController : NetworkBehaviour, ICommandHandler {
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

    public void Start() {
        seeker = GetComponent<Seeker>();
        body2D = GetComponent<Rigidbody2D>();
        name = "Ship " + idIndex++;
    }

    private void OnPathComplete(Path path) {
        if(!path.error) {
            this.path = path;
            nodeIndex = 0;
        }
    }

    private void FixedUpdate() {
        //i will have to eventually have to rewrite this to make a bit more sense
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

    [ClientRpc]
    public void RpcHandleCommand(int command, object commandData) {
        Debug.Log(commandData);
        HandleCommand(command, commandData);
    }

    public void HandleCommand(int command, object commandData) {
        SetDestination((Vector3)commandData);
    }

    public void SetDestination(Vector2 destination) {
        targetDestination = destination;
        seeker.StartPath(transform.position, targetDestination, OnPathComplete);
    }

    private void OnDestroy() {
        //used for cleanup
    }

    //make this more generic and add it to pixel math
    private Quaternion RotateTowards(Transform trans, Vector2 point) {
        float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        return Quaternion.Slerp(trans.rotation, q, Time.deltaTime * rotationSpeed);
    }
}
