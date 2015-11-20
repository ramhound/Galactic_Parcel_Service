using UnityEngine;
using System.Collections;

public class MatchZoom : MonoBehaviour {
    public tk2dCamera ourCamera;
    public tk2dCamera camToMatch;
    public bool shouldMatch;

    private void Update() {
        if(shouldMatch && (ourCamera != null && camToMatch != null))
            ourCamera.ZoomFactor = camToMatch.ZoomFactor;
    }
}
