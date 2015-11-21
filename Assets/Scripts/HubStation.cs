using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HubStation : Location {
    public HubUiManager hubUiManager;
    public GameObject hubMenu;
    public GameObject[] spawnables;
    public List<Package> packages = new List<Package>();

    public override void Start() {
        base.Start();
        if(!NetworkClient.active || isServer) {
            packages.Add(new Package() {
                fragility = 1,
                sender = ClientManager.farnsberg,
                receiver = ClientManager.GenerateClient(clientStyles),
                size = Vector2.one
            });
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

    public override void OnGameTick() {
        //packageneration here
        if(GameTimer.currentTick == 1)
            GamePlayer.localInstance.DisplayBanner(new Vector2(-1, 0), "Why not farnsberg?", Banner.BannerType.Package);

    }

    public override void HandleCommand(CommandPacket packet) {
        base.HandleCommand(packet);

        var ship = Instantiate(spawnables[(int)(packet.commandData.x)]) as GameObject;
        if(isServer) NetworkServer.Spawn(ship);
    }

    private Package CreatePackage() {
        var package = new Package() {
            sender = ClientManager.GenerateClient(clientStyles),
            receiver = ClientManager.GenerateClient(clientStyles),
            fragility = 1f,
            size = Vector2.one
        };

        PopUp.DisplayBanner(package.receiver.profilePic, "Reveiver", Banner.BannerType.Message);
        return package;
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
    }
}
