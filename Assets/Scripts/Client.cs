using UnityEngine;
using System.Collections;

public class Client {
    public string name;
    public Sprite profilePic;
    public Location location;
    public Vector2 profilePicIndex;

    //called on the sender client when the package arrives
    public void PackageDelivered() {
        //not wokring yet
    }
}