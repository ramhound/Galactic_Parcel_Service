using UnityEngine;
using System.Collections;

public enum GameCommand { None, Move, Spawn, Build }
public interface ICommandHandler {
    void OnGameTick();
    void HandleCommand(CommandPacket packet);
}
public struct CommandPacket {
    public string playerId;
    public string[] uuids;
    public int command;
    public Vector3 commandData;
}


