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
    public GameObject shipPrefab;
    public CameraFollow camFollower;
    //private GameNetworkHandler networkHandler;

    //private List<ICommandHandler> units = new List<ICommandHandler>();
    public string[] uuids;
    private static int idIndex = 0;

    private void Awake() {
        name = playerId + idIndex++;
        localInstance = this;
    }

    private void Start() {
        StartGameTimer();
    }

    //make a packet merger function
    public struct CommandPacket {
        public string playerId;
        public string[] uuids;
        public int command;
        public object commandData;
    }

    private void Update() {
        if(connectionToServer == null || isLocalPlayer) {
            if(Input.GetMouseButtonDown(0) && uuids.Length > 0) {
                var click = Input.mousePosition;
                var pos = Camera.main.ScreenToWorldPoint(click);

                var packet = new CommandPacket() {
                    playerId = name,
                    uuids = this.uuids,
                    command = (int)PlayerCommand.Move,
                    commandData = pos
                };

                SendCommandPacket(packet);
            }

            if(Input.GetKeyUp(KeyCode.Escape))
                camFollower.SetMainTarget(null);
        }
    }

    private void SendCommandPacket(CommandPacket packet) {
        if(isLocalPlayer) CmdHandleCommandPacket(packet);
        else HandleCommandPacket(packet);
    }

    [Command]
    public void CmdHandleCommandPacket(CommandPacket packet) {
        HandleCommandPacket(packet);
    }

    private void HandleCommandPacket(CommandPacket packet) {
        if(packet.uuids.Length > 0) {
            foreach(var s in packet.uuids) {
                var unit = GameObject.Find(s).GetComponent<ICommandHandler>();
                if(isServer) unit.RpcHandleCommand(packet.command, packet.commandData);
                else unit.HandleCommand(packet.command, packet.commandData);
            }
        }
    }

    private void StartGameTimer() {
        if(connectionToServer == null || isServer) {
            GameTimer.StartTimer();
        }
    }
}
