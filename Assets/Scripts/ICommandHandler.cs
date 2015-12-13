using UnityEngine;
using System.Collections;

public enum GameCommand { None, Move, Spawn, Build, PickUp, Delivery, Shuttle, Return }
public interface ICommandHandler {
    void OnGameTick();
    void ExecuteCommand(GameCommand command);
    void CompletedCommand(GameCommand command);
    void ReceiveCommand(CommandPacket packet);
}
public struct CommandPacket {
    public GameCommand command;
    public string[] uuids;
    public string dataString;
    public Vector3 dataVector;
}


