using UnityEngine;
using System.Collections;

public class SpawnStuff : MonoBehaviour {
    public GameObject[] spawnStuff;
    public bool spawn = true;

    private void Start() {
        foreach(var g in spawnStuff)
            g.SetActive(spawn);
    }
}
