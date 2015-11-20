using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Location : NetworkBehaviour, ICommandHandler, ISelectable {
    [BitMask(typeof(ClientStyle))]
    public ClientStyle clientStyles = ClientStyle.British;
    private Transform trans;
    private SpriteRenderer rend;
    public float rotationSpeed;
    public bool rotateLeft;
    public int spawnRate = 1;

    private void Awake() {
        trans = transform;
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if(GamePlayer.localInstance.connectionToServer == null || isServer)
            GameTimer.onGameTick += OnGameTick;
    }

    public virtual void OnGameTick() {
        if(clientStyles != 0 && GameTimer.currentTick % spawnRate == 0) {
            ClientManager.CreateClient(clientStyles);
        }
    }

    private void Update() {
        Rotate();
    }

    public virtual void OnClick() {
        SetSelected(true);
    }

    public virtual void SetSelected(bool selected) {
        if(selected) {
            GamePlayer.localInstance.SetSelectedUnit(this);
            Camera.main.GetComponent<CameraFollow>().SetMainTarget(trans);
        }
    }

    public virtual void GenerateClient() {

    }

    public virtual void HandleCommand(int command, object commandData) {
        
    }

    [ClientRpc]
    public virtual void RpcHandleCommand(int command, object commandData) {
        
    }

    private void Rotate() {
        var rot = trans.rotation.eulerAngles;
        trans.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + (rotateLeft ? (rotationSpeed * Time.deltaTime) : -(rotationSpeed * Time.deltaTime)));
    }
}