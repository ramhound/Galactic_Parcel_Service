using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ClientLocation : NetworkBehaviour, ICommandHandler {
    [BitMask(typeof(ClientStyle))]
    public ClientStyle clientStyles = ClientStyle.British;
    private Transform trans;
    private SpriteRenderer rend;
    public CameraFollow camFollower;
    public float rotationSpeed;
    public bool rotateLeft;
    public bool spawnClients = false;
    public int spawnRate = 6;

    //public int reputation


    private void Awake() {
        trans = transform;
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if(GamePlayer.localInstance.connectionToServer == null || isServer)
            GameTimer.onGameTick += OnGameTick;
    }

    public virtual void OnGameTick() {
        if(spawnClients && GameTimer.currentTick % spawnRate == 0) {
            ClientManager.CreateClient(clientStyles);
        }
    }

    private void Update() {
        Rotate();
    }

    public virtual void OnClick() {
        SetSelected();
    }

    public virtual void SetSelected() {
        camFollower.SetMainTarget(trans);
    }

    public virtual void GenerateClient() {

    }

    [ClientRpc]
    public virtual void RpcGenerateClient() {

    }

    public void HandleCommand(int command, object commandData) {
        
    }

    [ClientRpc]
    public void RpcHandleCommand(int command, object commandData) {
        
    }

    private void Rotate() {
        var rot = trans.rotation.eulerAngles;
        trans.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + (rotateLeft ? (rotationSpeed * Time.deltaTime) : -(rotationSpeed * Time.deltaTime)));
    }
}