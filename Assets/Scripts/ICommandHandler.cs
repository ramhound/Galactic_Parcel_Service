using System.Collections;

public enum PlayerCommand { None, Move, Spawn, Build }
public interface ICommandHandler {
    void RpcHandleCommand(int command, object commandData);
    void HandleCommand(int command, object commandData);
}
public struct CommandPacket {
    public string playerId;
    public string[] uuids;
    public int command;
    public object commandData;
}


