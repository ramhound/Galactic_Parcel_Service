using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GameCommandHandler : NetworkBehaviour, ICommandHandler {
    public int commandRate = 1;
    [SyncVar]
    public GameCommand currentCommand = GameCommand.None;
    [SyncVar]
    public string dataString;
    [SyncVar]
    public Vector3 dataVector;

    public virtual void Awake() {

    }

    public virtual void Start() {
        if(!NetworkClient.active || isServer) {
            GameTimer.AddGameCmdHandler(this);
        }
    }

    public virtual void OnGameTick() {
        //do something on game tick
        ExecuteCommand(currentCommand);
    }

    public virtual void ExecuteCommand(GameCommand command) {
        if(command != GameCommand.None && command != currentCommand) {
            //pause current command, do new, resume old...maybe
        }
    }

    public virtual void CompletedCommand(GameCommand command) {
        Debug.Log(name + " Completed " + command);
        currentCommand = GameCommand.None;
    }

    public virtual void ReceiveCommand(CommandPacket packet) {
        currentCommand = packet.command;
        dataString = packet.dataString;
        dataVector = packet.dataVector;
    }
}
