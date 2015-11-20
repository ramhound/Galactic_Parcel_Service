using UnityEngine;
using System.Collections;

public class BurnOff : MonoBehaviour {
    private Transform _trans;
    public Rigidbody2D _body2d;
    public float _minScale = 1f;

    private void Awake() {
        _trans = transform;
    }

    // Update is called once per frame
    private void Update() {
        if(_body2d != null) {
            float v = Mathf.Max(Mathf.Abs(_body2d.velocity.x), Mathf.Abs(_body2d.velocity.y));
            _trans.localScale = new Vector2(Mathf.Max(v * 1.3f, _minScale), _trans.localScale.y);
        }
    }
}
