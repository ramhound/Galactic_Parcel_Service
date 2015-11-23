using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameNetworkManager : NetworkManager {
    public static GameNetworkManager instance;
    public bool startOnStart = false;
    private bool started = false;

     private void Start() {
        instance = this;
    }

    private void Update() {
        if(startOnStart && !started) {
            StartHost();
            started = true;
        }
    }

    public override void OnStartHost() {
        base.OnStartHost();
    }
}
