using UnityEngine;
using System.Collections;

public class SelectionCollider : MonoBehaviour {
    public Location location;

	// Use this for initialization
	void Start () {
        if(location == null)
            location = GetComponent<Location>();
	}

    private void OnClick() {
        GamePlayer.localInstance.SetSelectedUnits(location.transform);
    }
}
