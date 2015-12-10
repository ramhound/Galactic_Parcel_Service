using UnityEngine;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip loopClip;

    public void SinglePlayerClicked() {
        //open slide menu for loading worlds
        //Application.LoadLevel("Main Game"); //maybe use load level async for no delay. 
        //i am already used to manually managing scene objects
        //so it seems like it would be the best of both worlds
        GameNetworkManager.instance.StartHost();
        GameNetworkDiscovery.instance.Initialize();
        GameNetworkDiscovery.instance.StartAsServer();
    }

    public void MultiPlayerClicked() {
        //open slide menu for local discovery; still need to be designed
        GameNetworkDiscovery.instance.Initialize();
        GameNetworkDiscovery.instance.StartAsClient();
        GameNetworkDiscovery.instance.showGUI = true;
    }

    private void Update() {
        if(!audioSource.isPlaying) {
            audioSource.clip = loopClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
