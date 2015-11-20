using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxManager : MonoBehaviour {
    //private Transform _trans;
    private Vector3 _lastPos;
    public Transform _target;

    public static List<ParallaxObject> _parallaxedObjects;

    public bool _shouldFollow = true;

    private void Awake() {
        if(_parallaxedObjects == null) _parallaxedObjects = new List<ParallaxObject>();
    }

    private void Start() {
        //_trans = transform;

        if(_target == null) {
            Debug.LogWarning("Target is null");
            enabled = false;
        } else {
            _lastPos = _target.position;
        }
    }

    private void LateUpdate() {
        if(_shouldFollow && _target.position != _lastPos) {
            var delta = _lastPos - _target.position;
            for(int i = 0; i < _parallaxedObjects.Count; i++)
                _parallaxedObjects[i].UpdatePara(delta);
            _lastPos = _target.position;
        }
    }

    public void SetTarget(Transform target) {
        this._target = target;
        if(_target != null) {
            _lastPos = _target.position;
            enabled = true;
        } else enabled = false;
    }
}
