using UnityEngine;
using System.Collections;

public enum GameCommand { None, Move, Spawn, Build, PickUp, Deliver, Shuttle, Return }
public interface ICommandHandler {
    void OnGameTick();
    void ExecuteCommand(GameCommand command);
    void CompletedCommand(GameCommand command);
    void ReceiveCommand(CommandPacket packet);
}
public struct CommandPacket {
    public string senderId;
    public string[] uuids;
    public GameCommand command;
    public Vector3 commandData;
}


