using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float cameraSpeed = 1f;
    private Vector2 startPos;
    private bool shouldTrack = false;

    public float xThresh = 10f;
    public float yThresh = 10f;

    private void Update() {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var delta = new Vector2(x, y);

        if(Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(0)) {
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shouldTrack = true;
        } else if(Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(0)) shouldTrack = false;

        if(shouldTrack) {
            delta = -(Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + startPos;
        }

        if(Mathf.Abs(delta.x) > xThresh || Mathf.Abs(delta.y) > yThresh) {
            GamePlayer.localInstance.SetSelectedUnits();

            transform.Translate(delta * Time.deltaTime * cameraSpeed);
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
