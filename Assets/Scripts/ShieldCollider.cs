using UnityEngine;
using System.Collections;

public class ShieldCollider : MonoBehaviour {
    public Location location;

	// Use this for initialization
	void Start () {
	    if(location == null) {
            location = GetComponent<Location>();
        }
	}

    private void OnTriggerEnter2D(Collider2D col) {
        //stuff with shield
    }
}
