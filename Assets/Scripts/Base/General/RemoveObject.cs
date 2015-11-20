using UnityEngine;
using System.Collections;

public class RemoveObject : MonoBehaviour {
    public float life = 3.0f;

    // Update is called once per frame
    void Update() {
        life -= Time.deltaTime;
        if(life <= 0.0f) {
            Destroy(gameObject);
        }
    }
}
