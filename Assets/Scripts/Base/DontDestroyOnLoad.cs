using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {
    public bool dontDestroyOnLoad = true;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
