using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HubStation : ClientLocation {
    public List<Package> packagaes = new List<Package>();

    //private void Start() {
    //    if(GamePlayer.localInstance.connectionToServer == null || isServer)
    //        GameTimer.onGameTick += OnGameTick; 
    //}

    public override void OnClick() {
        GamePlayer.localInstance.uuids = new string[] { };

        SetSelected();
    }

    public override void OnGameTick() {
        base.OnGameTick();
    }
}
