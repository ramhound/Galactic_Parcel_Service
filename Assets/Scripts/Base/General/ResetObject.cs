using UnityEngine;
using System.Collections;

public class ResetObject : MonoBehaviour {
    public enum Function { Destroy, Reset }
    public Function function = Function.Reset;
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D col) {
        if(function == Function.Reset) {
            if(destination != null) col.gameObject.transform.position = destination.position;
            else col.gameObject.transform.position = Vector2.zero;
        } else Destroy(col.gameObject);
    }
}
