using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameTimer : MonoBehaviour {
public delegate void GameTick();
    private static GameTimer instance;
    private static bool stopRequested = false;
    public static GameTick onGameTick;
    public static int currentTick = 0;
    public float tickRate = .1f;

    private void Awake() { instance = this; }

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
            if(onGameTick != null) onGameTick();
        }
    }
}
