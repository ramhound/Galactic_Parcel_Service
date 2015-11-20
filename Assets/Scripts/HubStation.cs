using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HubStation : Location {
    public HubUiManager hubUiManager;
    public GameObject hubMenu;
    public GameObject[] spawnables;
    //private void Start() {
    //    if(GamePlayer.localInstance.connectionToServer == null || isServer)
    //        GameTimer.onGameTick += OnGameTick; 
    //}

    public override void OnClick() {
        GamePlayer.localInstance.uuids = new string[] { };

        SetSelected(true);
    }

    public override void SetSelected(bool selected) {
        base.SetSelected(selected);

        hubMenu.gameObject.SetActive(selected);
        hubUiManager.selectedStation = selected ? this : null;
    }

    public override void OnGameTick() {
        base.OnGameTick();
    }

    public override void HandleCommand(int command, object commandData) {
        if(command == (int)PlayerCommand.Spawn) {

            var ship = Instantiate(spawnables[0]) as GameObject;
            if(isServer) NetworkServer.Spawn(ship);
        }
    }

    [ClientRpc]
    public override void RpcHandleCommand(int command, object commandData) {
        HandleCommand(command, commandData);
    }
}
