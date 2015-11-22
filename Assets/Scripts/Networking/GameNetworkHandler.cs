using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameNetworkHandler : NetworkManagerHUD {
    public static GameNetworkHandler instance;

     private void Start() {
        instance = this;
    }
}
