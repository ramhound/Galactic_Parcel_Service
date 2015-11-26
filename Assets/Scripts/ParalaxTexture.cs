using UnityEngine;
using System.Collections;

public class ParalaxTexture : MonoBehaviour {
    public Transform target;
    public float yPara = .5f;
    public float xPara = .5f;
    public static int speedFactor = 1;
    public bool useMouse = false;

    private void Start() {
        if(useMouse) {
            target = new GameObject().transform;
            target.position = Input.mousePosition;
        }
    }

    private void LateUpdate() {
        if(useMouse) target.position = Input.mousePosition;
        var rend = GetComponent<MeshRenderer>();
        var mat = rend.material;
        var sf = speedFactor * Camera.main.GetComponent<tk2dCamera>().ZoomFactor;
        mat.mainTextureOffset = new Vector2(target.position.x * (xPara / 100) * sf, target.position.y * (yPara / 100) * sf);
    }
}
