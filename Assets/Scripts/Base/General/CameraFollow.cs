using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow instance;
    public tk2dCamera cam { get; private set; }
    private Transform us;
    public Transform[] targets;
    public float followSpeed = 1.0f;
    public float zoomSpeedOut = 0.2f;
    public float zoomSpeedIn = 0.2f;
    public int maxTargets = 10;
    public bool allowZooming = true;
    public float minSpeedThreshhold = .2f;
    public float maxSpeedThreshhold = 3.0f;
    public float maxZoomFactor = 0.6f;
    private float minZoom;
    public Vector2 offSet = Vector2.zero;
    public bool battleView = false;
    public float battleZoomFactor = 0.5f;
    public float MAxDistanceThreshold = 5;
    public float battleZoomSpeed = 0.5f;
    public float maxBattleZoom = 0.4f; 

    private void Start() {
        instance = this;

        us = transform;
        cam = GetComponent<tk2dCamera>();
        minZoom = cam.ZoomFactor;
        targets = new Transform[maxTargets];
    }

    public void SetTargets(params Transform[] targets) {
        this.targets = targets;
        if(targets.Length == 1) {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", targets[0].position.x,
                "y", targets[0].position.y,
                "time", .5f,
                "easetype", iTween.EaseType.easeOutCubic
                ));
        } else if(targets.Length > 1) {
            //find center of all
        } else { }
    }

    private void FixedUpdate() {
        //if(targets.Length > 0 && targets[0] != null) {
        //    if(!battleView) {
        //        Vector3 start = us.position;
        //        Vector3 end = Vector3.MoveTowards(start, targets[0].position + (Vector3)offSet, followSpeed * Time.deltaTime);
        //        end.z = start.z;
        //        us.position = end;


        //        ////i want the camera to be quick for zoom out and slower for zoom in;
        //        //if(allowZooming && (body1 != null || body2D1 != null) && cam != null) {
        //        //    float speed = body2D1 != null ? body2D1.velocity.magnitude : body1.velocity.magnitude;
        //        //    float speedSclamp = Mathf.Clamp01( (speed - minSpeedThreshhold) / (maxSpeedThreshhold - minSpeedThreshhold));

        //        //    float targetZoom = Mathf.Lerp(minZoom, maxZoomFactor, speedSclamp);
        //        //    float zoom = Mathf.MoveTowards(cam.ZoomFactor, targetZoom, (targetZoom < cam.ZoomFactor ? zoomSpeedOut : zoomSpeedIn) * Time.deltaTime);
        //        //    cam.ZoomFactor = zoom;
        //        //}
        //    } else if(targets.Length > 1) {
        //        //TODO: find the center between all targets and center on that if possible

        //        ////find middle between 2 targets and center the camera on that
        //        //Vector3 start = us.position;
        //        //Vector3 end = Vector3.MoveTowards(start, ((target1.position + target2.position) * 0.5f) + (Vector3)offSet, followSpeed * Time.deltaTime);
        //        //end.z = start.z;
        //        //us.position = end;

        //        ////the zoom of the camera depends on the distance between the 2 targets
        //        //if(allowZooming) {
        //        //    float distance = Vector3.Distance(target2.position, target1.position);
        //        //    //float distanceClamp = Mathf.Clamp01((distance - minSpeedThreshhold) / (maxSpeedThreshhold - minSpeedThreshhold));
        //        //    //float targetZoom = Mathf.Lerp(minZoom, maxZoomFactor, distanceClamp);

        //        //    //var zoom = Mathf.MoveTowards(cam.ZoomFactor, targetZoom, (targetZoom < cam.ZoomFactor ? zoomSpeedOut : zoomSpeedIn) * Time.deltaTime);

        //        //    cam.ZoomFactor = Mathf.Max(maxBattleZoom, Mathf.Clamp01(Mathf.MoveTowards(cam.ZoomFactor, distance / (Mathf.Pow(distance, 2) / battleZoomFactor), battleZoomSpeed * Time.deltaTime)));
        //        //}
        //    }
        //}
    }
}
