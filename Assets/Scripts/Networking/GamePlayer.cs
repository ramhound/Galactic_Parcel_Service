using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;

public class GamePlayer : NetworkBehaviour {
    private static GamePlayer _instance;
    //private static NetworkClient networkClient;
    public static GamePlayer localInstance {
        get { return _instance; }
        private set { _instance = value; }
    }
    public string playerId = "Krooked590";
    private string[] _uuids = new string[] { };
    public string[] uuids {
        get { return _uuids; }
        set {
            _uuids = value;
            //somethuing might need to go here not dsure et
        }
    }

    private ISelectable[] selectedUnits = new ISelectable[] { };
    private static int idIndex = 0;

    private void Start() {
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
                        senderId = name,
                        uuids = this.uuids,
                        command = (int)GameCommand.Move,
                        commandData = pos
                    };

                    SendCommandPacket(packet);
                } else {

                }
            }

            if(Input.GetKeyUp(KeyCode.Escape))
                SetSelectedUnits(new ISelectable[] { });
        }
    }

    public void SendCommandPacket(CommandPacket packet) {
        if(NetworkClient.active) CmdHandleCommandPacket(packet); //handle as network
        else HandleCommandPacket(packet); //or as single player
    }

    [Command]
    public void CmdHandleCommandPacket(CommandPacket packet) {
        HandleCommandPacket(packet);
    }

    //this is implied to be run on the server and single player if you follow it
    private void HandleCommandPacket(CommandPacket packet) {
        if(packet.uuids.Length > 0) {
            foreach(var s in packet.uuids) {
                var unit = GameObject.Find(s).GetComponent<GameCommandHandler>();
                unit.HandleCommand(packet);
            }
        }
    }

    public void SetSelectedUnits(ISelectable[] selections) {
        if(selectedUnits.Length > 0) {
            foreach(var u in selectedUnits)
                u.SetSelected(false);
        }
        selectedUnits = selections;
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
        Debug.Log(isLocalPlayer);
    }
}
