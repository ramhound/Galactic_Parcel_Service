using UnityEngine;
using System.Collections;

public class DeliveryZoneChecker : MonoBehaviour {
    public HubStation hubStation;

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Location") {
            hubStation.deliveryLocations.Add(col.GetComponent<Location>());
        }
    }
}
