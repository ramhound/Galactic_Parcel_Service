using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameTimer : NetworkBehaviour {
    public delegate void GameTick();
    private static GameTimer instance;
    private static bool stopRequested = false;
    public static GameTick onGameTick;
    public static int currentTick = 0;
    public static List<GameCommandHandler> gameCmdHandlers = new List<GameCommandHandler>();
    public float tickRate = .2f;

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
