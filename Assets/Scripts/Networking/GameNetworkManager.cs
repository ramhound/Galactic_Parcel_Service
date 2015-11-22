using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameNetworkManager : NetworkManager {
    public static GameNetworkManager instance;
    public bool startOnStart = false;

     private void Start() {
        instance = this;

        if(startOnStart) {
            StartHost();
        }
    }
}
