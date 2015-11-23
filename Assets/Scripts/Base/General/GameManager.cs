using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public bool dontDestroyOnLoad = true;

    private void Awake() {
        if(dontDestroyOnLoad) {
            if(instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            } else { Destroy(gameObject); }
        } else instance = this;
    }

    private void Start() {

    }
}
