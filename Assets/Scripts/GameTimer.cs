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
    private static Dictionary<PlayerCommandHandler, int> playerCmdHandlers = new Dictionary<PlayerCommandHandler, int>();
    public float tickRate = .2f;

    private void Awake() {

    }

    private void Start() {
        if(!NetworkClient.active || isServer) {
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
            foreach(var e in playerCmdHandlers) {
                if(currentTick % e.Value == 0) {
                    e.Key.OnGameTick();
                }
            }
        }
    }

    public static void AddPlayerCmdHandler(PlayerCommandHandler pch, int eTick) {
        playerCmdHandlers.Add(pch, eTick);
    }

    public static void RemovePlayerCmdHandler(PlayerCommandHandler pch) {
        playerCmdHandlers.Remove(pch);
    }
}
