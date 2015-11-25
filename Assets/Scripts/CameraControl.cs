using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float cameraSpeed = 1f;
    public CameraFollow camFollower;
    private Vector2 startPos;
    private bool shouldTrack = false;

    private void Update() {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var delta = new Vector2(x, y);

        if(Input.GetMouseButtonDown(2) || ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButtonDown(0))) {
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shouldTrack = true;
        } else if(Input.GetMouseButtonUp(2) || (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)) || Input.GetMouseButtonUp(0)) shouldTrack = false;

        if(shouldTrack) {
            delta = -(Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + startPos;
        }

        if(delta.x != 0 || delta.y != 0) {
            GamePlayer.localInstance.SetSelectedUnits();

            transform.Translate(delta * Time.deltaTime * cameraSpeed);
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
