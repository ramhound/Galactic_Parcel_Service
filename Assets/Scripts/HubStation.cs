using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HubStation : Location {
    public HubUiManager hubUiManager;
    public GameObject hubMenu;
    public GameObject[] spawnables;
    public List<Package> packages = new List<Package>();

    private void Start() {
        if(GamePlayer.localInstance.connectionToServer == null || isServer) {
            GameTimer.onGameTick += OnGameTick;
        }
    }

    public override void OnClick() {
        GamePlayer.localInstance.uuids = new string[] { };

        SetSelected(true);
    }

    public override void SetSelected(bool selected) {
        base.SetSelected(selected);

        hubMenu.gameObject.SetActive(selected);
        hubUiManager.selectedStation = selected ? this : null;
    }

    public void OnGameTick() {
        //packageneration here
        if(GameTimer.currentTick % spawnRate == 0) {
            packages.Add(CreatePackage());

        }
    }

    private Package CreatePackage() {
        var package = new Package() {
            sender = ClientManager.GenerateClient(clientStyles),
            receiver = ClientManager.GenerateClient(),
            fragility = 100f,
            size = Vector2.one
        };

        PopUp.DisplayBanner(package.receiver.profilePic, "Reveiver", Banner.BannerType.Message);
        return package;
    }

    //private void 

    public override void HandleCommand(int command, object commandData) {
        if(command == (int)PlayerCommand.Spawn) {

            var ship = Instantiate(spawnables[(int)commandData]) as GameObject;
            if(isServer) NetworkServer.Spawn(ship);
        }
    }

    [ClientRpc]
    public override void RpcHandleCommand(int command, object commandData) {
        HandleCommand(command, commandData);
    }
}
