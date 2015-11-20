using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Ship : NetworkBehaviour {
    public enum ShipType { Cargo, Shuttle, Speed, Defense, Special, Attack, Enemy_Attack, Enemy_Speed, Enemy_Defense }
    public ShipType shipType = ShipType.Cargo;

    
}
public class ShipStats {
    public int health;
    public int defense;
    public int speed;
    public int cargoSpace;
}
