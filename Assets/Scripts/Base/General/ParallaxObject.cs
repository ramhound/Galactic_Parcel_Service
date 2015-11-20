using UnityEngine;
using System.Collections;

public class ParallaxObject : MonoBehaviour {
    private Transform _trans;
    [Range(0f, 1f)]
    public float _xPara = .5f;
    [Range(0f, 1f)]
    public float _yPara = .5f;

    private void Start() { 
        _trans = transform;
        if(!ParallaxManager._parallaxedObjects.Contains(this)) {
            ParallaxManager._parallaxedObjects.Add(this);
            gameObject.layer = LayerMask.NameToLayer("Background");
        }
    }

    public void UpdatePara(Vector2 delta) {
        _trans.position += new Vector3(delta.x * _xPara, delta.y * _yPara, 0f);
    }
}
