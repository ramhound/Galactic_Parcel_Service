using UnityEngine;
using System.Collections;

public class ZoomCamera : MonoBehaviour {
    public tk2dCamera cam;
    public float zoomSensitivity = 15.0f;
    public float zoomSpeed = 5.0f;
    public float zoomMin = .2f;
    public float zoomMax = 5.0f;     
    private float zoom;

    public void Start() {
        UICamera.onScroll += OnScroll;
        zoom = cam.ZoomFactor;
    }

    private void Update() {
        //need to test but dont have a device yet
        if(Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            OnScroll(gameObject, -deltaMagnitudeDiff / 100f);
        }
    }

    public void OnScroll(GameObject go, float delta) {
        zoom = cam.ZoomFactor;
        zoom += delta * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
    }

    private void LateUpdate() {
        cam.ZoomFactor = Mathf.MoveTowards(cam.ZoomFactor, zoom, Time.deltaTime * zoomSpeed);
    }

    private void OnDestroy() {
        UICamera.onScroll -= OnScroll;
    }
}
