using UnityEngine;
using System.Collections;

public enum GameCommand { None, Move, Spawn, Build, PickUp, Deliver }
public interface ICommandHandler {
    void OnGameTick();
    void HandleCommand(CommandPacket packet);
}
public struct CommandPacket {
    public string senderId;
    public string[] uuids;
    public int command;
    public Vector3 commandData;
}


