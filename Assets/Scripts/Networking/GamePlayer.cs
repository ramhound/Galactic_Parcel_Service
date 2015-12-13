using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;

public class GamePlayer : GameCommandHandler {
    private static GamePlayer _instance;
    //private static NetworkClient networkClient;
    public static GamePlayer localInstance {
        get { return _instance; }
        private set { _instance = value; }
    }
    public string playerId = "Krooked590";
    public string[] uuids = new string[] { };

    private Transform[] selectedUnits = new Transform[] { };
    private static int idIndex = 0;

    public override void Start() {
        base.Start();
        if(!NetworkClient.active)
            localInstance = this;
        name = playerId + idIndex++;
    }

    //make a packet merger function

    private void Update() {
        if(!NetworkClient.active || isLocalPlayer) {
            if(Input.GetMouseButtonDown(0) && uuids.Length > 0) {
                var click = Input.mousePosition;
                var pos = Camera.main.ScreenToWorldPoint(click);

                var hit = Physics2D.Raycast(pos, Vector2.zero);
                if(hit.collider == null) {
                    var packet = new CommandPacket() {
                        dataString = name,
                        uuids = this.uuids,
                        command = GameCommand.Move,
                        dataVector = pos
                    };

                    SendCommandPacket(packet);
                }
            }
        }
    }

    public void SendCommandPacket(CommandPacket packet) {
        if(NetworkClient.active) CmdReceiveCommand(packet); //handle as network
        else ReceiveCommand(packet); //or as single player
    }

    [Command]
    public void CmdReceiveCommand(CommandPacket packet) {
        ReceiveCommand(packet);
    }

    //this is implied to be run on the server and single player if you follow it
    public override void ReceiveCommand(CommandPacket packet) {
        base.ReceiveCommand(packet);

        if(packet.uuids.Length > 0) {
            foreach(var s in packet.uuids) {
                var unit = GameObject.Find(s).GetComponent<GameCommandHandler>();
                unit.ReceiveCommand(packet);
            }
        }
    }

    public void SetSelectedUnits(params Transform[] selections) {
        if(selectedUnits.Length > 0) {
            foreach(var u in selectedUnits)
                u.GetComponent<ISelectable>().SetSelected(false);
        }

        if(selections.Length > 0) {
            foreach(var u in selections)
                u.GetComponent<ISelectable>().SetSelected(true);
        }
        selectedUnits = selections;

        //camera should update here
        Camera.main.GetComponent<CameraFollow>().SetTargets(selectedUnits);
    }

    public void DisplayBanner(Vector2 characterIndex, string text, Banner.BannerType bannerType) {
        if(isServer) RpcDisplayBanner(characterIndex, text, bannerType);
        else PopUp.DisplayBanner(ClientManager.GenerateClient(characterIndex).profilePic, text, bannerType);
    }

    [ClientRpc]
    public void RpcDisplayBanner(Vector2 characterIndex, string text, Banner.BannerType bannerType) {
        PopUp.DisplayBanner(ClientManager.GenerateClient(characterIndex).profilePic, text, bannerType);
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        localInstance = this;
    }
}
