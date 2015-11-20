using UnityEngine;
using System.Collections;

public class ChildRotationStopper : MonoBehaviour {
    private Transform _trans;

    private void Awake() { _trans = transform; }

    private void LateUpdate() { _trans.rotation = Quaternion.Euler(Vector3.zero); }
}
