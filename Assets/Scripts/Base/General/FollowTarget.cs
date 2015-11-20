using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {
    public Transform target;
    public bool shouldFollow;
    private Transform us;
    private Vector3 delta;

	private void Start () {
        Init();
	}

    private void Init() {
        us = transform;
    }

	private void Update () {
        if(shouldFollow && target != null) us.position = target.position - delta;
	}

    public FollowTarget SetTarget(Transform target) {
        this.target = target;
        RecalculateDelta();
        return this;
    }

    public FollowTarget RecalculateDelta() {
        if(target != null) {
            delta = target.position - us.position;
            enabled = true;
        } else {
            Debug.LogWarning("The target is null");
            enabled = false;
        }
        return this;
    }
}
