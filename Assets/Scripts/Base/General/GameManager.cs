using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GamePlayer owner { get; private set; }
    public static bool _isPaused;
    public static Sound _bgMusic;
    public static Sound _combatStart;
    public static Sound _combatLoop;
    private bool _init;

    private void Awake() {
        //_bgMusic = SoundManager.Play("BG Music").Loop(true);
    }

    public static void SetOwner(GamePlayer owner) {
        GameManager.owner = owner;
    }
}
