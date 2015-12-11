using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceMarker : MonoBehaviour {
    public Ship selectedShip;
    public Image circle;
    public Image packageIcon;
    public Text packageCountLabel;
    public Text distanceLabel;
    public Location loc;
    public int packageCount;

    public Color[] _colors = new Color[] { Color.red, Color.yellow, Color.green };
    public static Color[] colors;

    private void Update() {
        //distanceLabel.text = "" + Vector2.Distance(selectedShip.transform.position, loc.position);

        Vector3 v3Pos = Camera.main.WorldToViewportPoint(loc.position);

        if(v3Pos.x >= 0.0f && v3Pos.x <= 1.0f && v3Pos.y >= 0.0f && v3Pos.y <= 1.0f)
            return; // Object center is visible

        v3Pos.x -= 0.5f;  // Translate to use center of viewport
        v3Pos.y -= 0.5f;
        v3Pos.z = 0;      // I think I can do this rather than do a 
        //   a full projection onto the plane

        float fAngle = Mathf.Atan2(v3Pos.x, v3Pos.y);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fAngle * Mathf.Rad2Deg);

        v3Pos.x = 0.42f * Mathf.Sin(fAngle) + 0.5f;  // Place on ellipse touching 
        v3Pos.y = 0.42f * Mathf.Cos(fAngle) + 0.5f;  //   side of viewport
        v3Pos.z = Camera.main.nearClipPlane + 0.01f;  // Looking from neg to pos Z;

        var locd = Camera.main.ViewportToWorldPoint(v3Pos);
        //transform.position = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(locd));
        transform.position = v3Pos;

        //change colour based on client package durability

        //code goes here somewhere
        //ColourMarkers();
        //PositionDistanceText();
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
}
