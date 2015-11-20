using UnityEngine;
using System.Collections;

public class ParalaxTexture : MonoBehaviour {
    public Transform target;
    [Range(0f, 1f)]
    public float yPara = .5f;
    [Range(0f, 1f)]
    public float xPara = .5f;
    public static int speedFactor = 1;

    private void LateUpdate() {
        var rend = GetComponent<MeshRenderer>();
        var mat = rend.material;
        mat.mainTextureOffset = new Vector2(target.position.x * (xPara / 100) * speedFactor, target.position.y * (yPara / 100) * speedFactor);
    }
}
