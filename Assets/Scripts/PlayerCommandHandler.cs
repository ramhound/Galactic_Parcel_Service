﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerCommandHandler : NetworkBehaviour, ICommandHandler {
    public int commandRate = 1;
    [SyncVar]
    public CommandPacket currentCommand;

    public virtual void Start() {
        if(!NetworkClient.active || isServer)
            GameTimer.AddPlayerCmdHandler(this, commandRate);
    }

    public virtual void OnGameTick() {
        //do something on game tick
    }

    public virtual void HandleCommand(CommandPacket packet) {
        currentCommand = packet;
    }
}