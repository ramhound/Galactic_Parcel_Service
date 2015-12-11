using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameTimer : NetworkBehaviour {
    public delegate void GameTick();
    public static GameTimer instance;
    public static GameTick onGameTick;
    public static List<GameCommandHandler> gameCmdHandlers = new List<GameCommandHandler>();

    private static bool stopRequested = false;
    public static int currentTick = 0;
    public float ticksPerSecond = 5f;
    private float tickRate = .2f;

    private void Start() {
        if(!NetworkClient.active) {
            instance = this;
            GameTimer.StartTimer();
        }
    }

    public static void StartTimer() {
        stopRequested = false;
        instance.StartCoroutine("Timer");
    }

    public static void StopTimer() {
        stopRequested = true;
        instance.StopCoroutine("Timer");
    }

    private IEnumerator Timer() {
        tickRate = 1f / ticksPerSecond;
        while(!stopRequested) {
            yield return new WaitForSeconds(tickRate);
            currentTick++;
            //if(onGameTick != null) onGameTick();
            foreach(var h in gameCmdHandlers) {
                if(currentTick % h.commandRate == 0) {
                    h.OnGameTick();
                }
            }
        }
    }

    public static void AddGameCmdHandler(GameCommandHandler h) {
        if(!gameCmdHandlers.Contains(h))
            gameCmdHandlers.Add(h);
    }

    public static void RemoveGameCmdHandler(GameCommandHandler h) {
        if(gameCmdHandlers.Contains(h))
            gameCmdHandlers.Remove(h);
    }

    public override void OnStartServer() {
        base.OnStartServer();
        instance = this;
        GameTimer.StartTimer();
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        if(isServer) {
            Debug.Log("Server asdjfka");
        }
    }
}
