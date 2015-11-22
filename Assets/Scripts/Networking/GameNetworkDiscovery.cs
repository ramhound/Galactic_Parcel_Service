using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameNetworkDiscovery : NetworkDiscovery {
    public static GameNetworkDiscovery instance;

    public void Start() {
        instance = this;
    }
}
