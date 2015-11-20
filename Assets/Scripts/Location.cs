using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Location : NetworkBehaviour, ICommandHandler, ISelectable {
    [BitMask(typeof(CharacterStyle))]
    public CharacterStyle clientStyles = CharacterStyle.British;
    private Transform trans;
    private SpriteRenderer rend;
    public float rotationSpeed;
    public bool rotateLeft;
    public int spawnRate = 60; //seconds
    public List<Client> clients = new List<Client>();

    private void Awake() {
        trans = transform;
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    //i do not want this tick going off on the local clients
    private void Start() {

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