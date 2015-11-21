using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameNetworkHandler : NetworkManagerHUD { 
     private void Start() {
        manager.StartHost();
    }
}
