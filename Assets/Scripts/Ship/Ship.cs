using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Ship : NetworkBehaviour, ISelectable {
    public enum ShipType { Cargo = 0, Shuttle, Speed, Defense, Special, Attack, Enemy_Attack, Enemy_Speed, Enemy_Defense }
    public ShipType shipType = ShipType.Cargo;

    public void SetSelected(bool selected) {
        if(selected) {
            GamePlayer.localInstance.SetSelectedUnit(this);
            GamePlayer.localInstance.uuids = new string[] { name };
            Camera.main.GetComponent<CameraFollow>().SetMainTarget(transform);
        }
    }

    public void OnClick() {
        SetSelected(true);
    }
}
public class ShipStats {
    public int health;
    public int defense;
    public int speed;
    public int cargoSpace;
}
