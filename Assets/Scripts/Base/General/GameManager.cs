using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public bool dontDestroyOnLoad = true;

    public AudioSource audioSource;

    private void Awake() {
        if(dontDestroyOnLoad) {
            if(instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            } else { DestroyImmediate(gameObject); }
        } else instance = this;
    }

    private void Start() {
        if(Application.loadedLevelName == "Main Game") {
            audioSource.volume = .1f;
            audioSource.Play();
        }
    }

    private void OnLevelWasLoaded(int level) {
        if(level == 1) {
            audioSource.volume = .1f;
            audioSource.Play();
        }
    }
}
