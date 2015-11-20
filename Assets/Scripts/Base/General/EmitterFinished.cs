using UnityEngine;
using System.Collections;

public class EmitterFinished : MonoBehaviour {
    public delegate void EmitterDelegate(Transform t);
    public EmitterDelegate onEmmiterFinished;

    public void OnDestroy() {
        if(onEmmiterFinished != null) {
            onEmmiterFinished(transform);
        }
    }
}
