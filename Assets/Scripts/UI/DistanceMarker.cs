using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceMarker : MonoBehaviour {
    public ShipUIManager shipUIManager;
    public Ship selectedShip;
    public Image circle;
    public Image packageIcon;
    public Text packageCountLabel;
    public Text distanceLabel;
    public Location loc;
    public Vector2 distanceFromShip = new Vector2(50, 500);
    public float rotationSpeed = 10;
    public int packageCount;

    public Color[] _colors = new Color[] { Color.red, Color.yellow, Color.green };
    public static Color[] colors;

    private void Update() {
        //distanceLabel.text = "" + Vector2.Distance(selectedShip.transform.position, loc.position);
        transform.rotation = RotateTowards(transform, loc.position);
        transform.localPosition = GetPosition() * Camera.main.GetComponent<tk2dCamera>().ZoomFactor;

        //change colour based on client package durability

        //code goes here somewhere
        //ColourMarkers();
        //PositionDistanceText();
    }

    public Vector2 GetPosition() {
        transform.localPosition = Vector2.zero;
        var distance = Vector2.Distance(selectedShip.transform.position, loc.position);
        var scale = Mathf.Clamp(distance * 100, distanceFromShip.x, distanceFromShip.y);
        var axis = (Vector3)loc.position - selectedShip.transform.position;
        return axis.normalized * scale;
    }

    public void MarkerClicked() {
        shipUIManager.PackageMarkerClicked(this);
    }

    //private void ColourMarkers() {
    //    if(_colors.Length == 0) return;

    //    float val = _clientDestination._client._packageDurability / 100f;
    //    val *= (_colors.Length - 1);
    //    int startIndex = Mathf.FloorToInt(val);

    //    Color c = _colors[0];

    //    if(startIndex >= 0) {
    //        if(startIndex + 1 < _colors.Length) {
    //            float factor = (val - startIndex);
    //            c = Color.Lerp(_colors[startIndex], _colors[startIndex + 1], factor);
    //        } else if(startIndex < _colors.Length) {
    //            c = _colors[startIndex];
    //        } else c = _colors[_colors.Length - 1];
    //    }

    //    _arrow.color = c;
    //    distanceLabel.color = c;
    //    _distance.color = c;
    //}

    private Quaternion RotateTowards(Transform trans, Vector2 point) {
        float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        return Quaternion.Slerp(trans.rotation, q, Time.deltaTime * rotationSpeed);
    }
}
