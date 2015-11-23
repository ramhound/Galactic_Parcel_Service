using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxManager : MonoBehaviour {
    //private Transform _trans;
    private Vector3 lastPos;
    public Transform target;
    public bool useMouse = false;

    public static List<ParallaxObject> _parallaxedObjects;

    private void Awake() {
        if(_parallaxedObjects == null) _parallaxedObjects = new List<ParallaxObject>();
        if(useMouse) {
            target = new GameObject().transform;
            target.position = Input.mousePosition;
        }
    }

    private void Start() {
        //_trans = transform;

        if(target == null) {
            Debug.LogWarning("Target is null");
            enabled = false;
        } else {
            lastPos = target.position;
        }
    }

    private void LateUpdate() {
        if(useMouse) target.position = Input.mousePosition;
        if(target.position != lastPos) {
            var delta = lastPos - target.position;
            for(int i = 0; i < _parallaxedObjects.Count; i++)
                _parallaxedObjects[i].UpdatePara(delta);
            lastPos = target.position;
        }
    }

    public void SetTarget(Transform target) {
        this.target = target;
        if(this.target != null) {
            lastPos = this.target.position;
            enabled = true;
        } else enabled = false;
    }
}
