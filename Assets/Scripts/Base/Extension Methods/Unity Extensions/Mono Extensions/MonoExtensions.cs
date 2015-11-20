using UnityEngine;
using System.Collections;

public static partial class MonoExtensions {
    public static string printWarning(this MonoBehaviour mono, object obj) {
        Debug.LogWarning(obj.ToString());
        return obj.ToString();
    }

    public static string printError(this MonoBehaviour mono, object obj) {
        return mono.printError(obj, false);
    }

    public static string printError(this MonoBehaviour mono, object obj, bool pause) {
        Debug.LogError(obj.ToString());
        if(pause) Debug.Break();
        return obj.ToString();
    }
}
