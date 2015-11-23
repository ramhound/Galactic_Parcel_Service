using UnityEngine;
using System.Collections;

public class ParallaxObject : MonoBehaviour {
    private Transform trans;
    public float xPara = .5f;
    public float yPara = .5f;

    private void Start() { 
        trans = transform;
        if(!ParallaxManager._parallaxedObjects.Contains(this)) {
            ParallaxManager._parallaxedObjects.Add(this);
            gameObject.layer = LayerMask.NameToLayer("Background");
        }
    }

    public void UpdatePara(Vector2 delta) {
        trans.position += new Vector3(delta.x * xPara, delta.y * yPara, 0f);
    }
}
