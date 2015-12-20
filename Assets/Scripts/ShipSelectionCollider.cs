using UnityEngine;
using System.Collections;

public class ShipSelectionCollider : MonoBehaviour {
    public Ship ship;

    private void OnClick() {
        ship.OnClick();
    }
}
