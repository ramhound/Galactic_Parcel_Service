using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;

public class GamePlayer : NetworkBehaviour {
    private static GamePlayer _instance;
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

    private ISelectable selectedUnit;
    private static int idIndex = 0;

    private void Awake() {
        name = playerId + idIndex++;
        localInstance = this;
    }

    private void Start() {

    }

    //make a packet merger function

    private void Update() {
        if(connectionToServer == null || isLocalPlayer) {
            if(Input.GetMouseButtonDown(0) && uuids.Length > 0) {
                var click = Input.mousePosition;
                var pos = Camera.main.ScreenToWorldPoint(click);

                var hit = Physics2D.Raycast(pos, Vector2.zero);
                if(hit.collider == null) {
                    var packet = new CommandPacket() {
                        playerId = name,
                        uuids = this.uuids,
                        command = (int)PlayerCommand.Move,
                        commandData = pos
                    };

                    SendCommandPacket(packet);
                } else {

                }
            }

            if(Input.GetKeyUp(KeyCode.Escape))
                Camera.main.GetComponent<CameraFollow>().SetMainTarget(null);
        }
    }

    public void SendCommandPacket(CommandPacket packet) {
        if(connectionToServer != null) CmdHandleCommandPacket(packet); //handle as network
        else HandleCommandPacket(packet); //or as single player
    }

    [Command]
    public void CmdHandleCommandPacket(CommandPacket packet) {
        Debug.Log(connectionToServer);
        HandleCommandPacket(packet);
    }

    private void HandleCommandPacket(CommandPacket packet) {
        if(packet.uuids.Length > 0) {
            foreach(var s in packet.uuids) {
                var unit = GameObject.Find(s).GetComponent<ICommandHandler>();
                unit.HandleCommand(packet.command, packet.commandData);

                //if(isServer) unit.RpcHandleCommand(packet.command, packet.commandData);
                //else unit.HandleCommand(packet.command, packet.commandData);
            }
        }
    }

    public void SetSelectedUnit(ISelectable selection) {
        if(selectedUnit != null) selectedUnit.SetSelected(false);
        selectedUnit = selection;
    }

    public void OnConnected() {
        Debug.Log("Test");
    }
}
