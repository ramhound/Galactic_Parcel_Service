using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VirtualKeyManager : MonoBehaviour {
    public static bool initialized;
    private static List<int> virtualKeyIndexers = new List<int>();
    private static VirtualKey[] virtualKeys;
    private static VirtualKey[] virtualKeysStaged;
    private static VirtualAxis[] virtualAxis;

    private void LateUpdate() {
        if(!initialized) Init(20);
        UpdateVirtualKeys();
    }

    public static void Init(int keyCount) {
        initialized = true;
        virtualKeys = new VirtualKey[keyCount];
        virtualKeysStaged = new VirtualKey[keyCount];
        virtualAxis = new VirtualAxis[keyCount];
    }

    public static void UpdateVirtualKeys() {
        for(int i = 0; i < virtualKeyIndexers.Count; i++) {
            virtualKeys[i].wasPressed = virtualKeysStaged[i].wasPressed;
            virtualKeysStaged[i].wasPressed = false;
            if(virtualKeys[i].wasPressed)
                virtualKeys[i].isHeld = true;

            virtualKeys[i].wasReleased = virtualKeysStaged[i].wasReleased;
            virtualKeysStaged[i].wasReleased = false;
            if(virtualKeys[i].wasReleased)
                virtualKeys[i].isHeld = false;

            if(virtualKeys[i].isHeld)
                virtualKeys[i].time = Mathf.Min(1f, virtualKeys[i].time + (virtualKeys[i].sensitivity * Time.deltaTime));
            else
                virtualKeys[i].time = Mathf.Max(0f, virtualKeys[i].time - (virtualKeys[i].gravity * Time.deltaTime));
        }
    }

    public static void SetVirtualKey(string keyName, float sensitivity = 3f, float gravity = 3f, float deadZone = 0.01f) {
        if(LookupVirtualKeyIndex(keyName) != -1) {
            Debug.LogWarning("That key is already defined: (" + keyName + ")");
            return;
        }

        int index = GetFirstOpenKeySpot();
        virtualKeys[index] = new VirtualKey(keyName, sensitivity, gravity, deadZone);
        virtualKeysStaged[index] = new VirtualKey(keyName, sensitivity, gravity, deadZone);
        virtualKeyIndexers.Add(index);
    }

    public static void RemoveVirtualKey(string keyName) {
        int index = LookupVirtualKeyIndex(keyName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return;
        }

        virtualKeys[index] = null;
        virtualKeysStaged[index] = null;
        FillOpenWithLastKey();
    }

    public static void SetVirtualAxis(string axisName, string positiveKey, string negativeKey = null) {
        if(LookupVirtualAxisIndex(axisName) != -1) {
            Debug.LogWarning("That axis is already defined: (" + axisName + ")");
            return;
        }

        virtualAxis[GetFirstOpenAxisSpot()] = new VirtualAxis(axisName, positiveKey, negativeKey);
    }

    public static void RemovevirtualAxis(string axisName) {
        int index = LookupVirtualAxisIndex(axisName);
        if(index < 0 || index >= virtualAxis.Length) {
            Debug.LogWarning("A virtual axis by that name can not be found");
            return;
        }

        virtualAxis[index] = null;
        FillOpenWithLastAxis();
    }

    public static string ChangeVirtualKey(string origKey, string newKeyName) {
        int index = LookupVirtualKeyIndex(origKey);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return string.Empty;
        }

        virtualKeysStaged[index].ChangeName(newKeyName);
        return virtualKeys[index].ChangeName(newKeyName);
    }

    public static string ChangeVirtualAxisName(string origAxis, string newAxisName) {
        int index = LookupVirtualAxisIndex(origAxis);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual axis by that name can not be found");
            return string.Empty;
        }

        return virtualAxis[index].ChangeName(newAxisName);
    }

    public static void ChangeVirtualAxisKeys(string axisName, string positiveKey, string negativeKey) {
        int index = LookupVirtualAxisIndex(axisName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual axis by that name can not be found");
            return;
        }

        virtualAxis[index].ChangeKeys(positiveKey, negativeKey);
    }

    public static void PressVirtualKey(string keyName) {
        int index = LookupVirtualKeyIndex(keyName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return;
        }

        virtualKeysStaged[index].wasPressed = true;
    }

    public static void ReleaseVirtualKey(string keyName) {
        int index = LookupVirtualKeyIndex(keyName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return;
        }

        virtualKeysStaged[index].wasReleased = true;
    }

    public static bool GetVirtualKeyUp(string keyName) {
        int index = LookupVirtualKeyIndex(keyName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return false;
        }
        return virtualKeys[index].wasReleased;
    }

    public static bool GetVirtualKeyDown(string keyName) {
        int index = LookupVirtualKeyIndex(keyName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return false;
        }
        return virtualKeys[index].wasPressed;
    }

    public static float GetVirtualKeyRaw(string keyName) {
        int index = LookupVirtualKeyIndex(keyName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return 0f;
        }
        return virtualKeys[index].isHeld ? 1f : 0f;
    }

    public static float GetVirtualKey(string keyName) {
        int index = LookupVirtualKeyIndex(keyName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual key by that name can not be found");
            return 0f;
        }
        return virtualKeys[index].time;
    }

    public static float GetVirtualAxisRaw(string axisName) {
        int index = LookupVirtualAxisIndex(axisName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual axis by that name can not be found");
            return 0f;
        }
        return virtualAxis[index].timeRaw;
    }

    public static float GetVirtualAxis(string axisName) {
        int index = LookupVirtualAxisIndex(axisName);
        if(index < 0 || index >= virtualKeys.Length) {
            Debug.LogWarning("A virtual axis by that name can not be found");
            return 0f;
        }
        return virtualAxis[index].time;
    }

    public static int LookupVirtualKeyIndex(string keyName) {
        if(!string.IsNullOrEmpty(keyName)) {
            for(int i = 0; i < virtualKeys.Length; i++) {
                if(virtualKeys[i] == null) break;
                else if(virtualKeys[i].name.Equals(keyName)) return i;
            }
        } return -1;
    }

    public static VirtualKey LookupVirtualKeyByName(string keyName) {
        if(!string.IsNullOrEmpty(keyName)) {
            for(int i = 0; i < virtualKeys.Length; i++) {
                if(virtualKeys[i] == null) break;
                else if(virtualKeys[i].name.Equals(keyName)) return virtualKeys[i];
            }
        } return null;
    }

    public static int LookupVirtualAxisIndex(string axisName) {
        if(!string.IsNullOrEmpty(axisName)) {
            for(int i = 0; i < virtualAxis.Length; i++) {
                if(virtualAxis[i] == null) break;
                else if(virtualAxis[i].name.Equals(axisName)) return i;
            }
        } return -1;
    }

    public static VirtualAxis LookupVirtualAxisByName(string axisName) {
        if(!string.IsNullOrEmpty(axisName)) {
            for(int i = 0; i < virtualAxis.Length; i++) {
                if(virtualAxis[i] == null) break;
                else if(virtualAxis[i].name.Equals(axisName)) return virtualAxis[i];
            }
        } return null;
    }

    private static int GetFirstOpenKeySpot() {
        for(int i = 0; i < virtualKeys.Length; i++) {
            if(virtualKeys[i] == null) return i;
        } return -1;
    }

    private static int GetFirstOpenAxisSpot() {
        for(int i = 0; i < virtualAxis.Length; i++) {
            if(virtualAxis[i] == null) return i;
        } return -1;
    }

    private static void FillOpenWithLastKey() {
        int spot = 0;
        for(int i = spot; i < virtualKeys.Length; i++) {
            if(virtualKeys[i] == null) {
                spot = i;
                for(int j = virtualKeys.Length; j > spot; j--) {
                    if(virtualKeys[j] != null) {
                        virtualKeys[spot] = virtualKeys[j];
                        virtualKeysStaged[spot] = virtualKeysStaged[j];

                        virtualKeys[j] = null;
                        virtualKeysStaged[j] = null;

                        virtualKeyIndexers.Remove(j);
                        return;
                    }
                } break;
            }
        }
    }

    private static void FillOpenWithLastAxis() {
        int spot = 0;
        for(int i = spot; i < virtualAxis.Length; i++) {
            if(virtualAxis[i] == null) {
                spot = i;
                for(int j = virtualAxis.Length; j > spot; j--) {
                    if(virtualAxis[j] != null) {
                        virtualAxis[spot] = virtualAxis[j];
                        virtualAxis[j] = null;
                        return;
                    }
                } break;
            }
        }
    }
}

public class VirtualKey {
    public string name { get; private set; }
    public float time { get; set; }
    public float sensitivity { get; set; }
    public float gravity { get; set; }
    public float deadZone { get; set; }
    public bool isHeld { get; set; }
    private bool _wasPressed;
    public bool wasPressed {
        get { return _wasPressed; }
        set {
            _wasPressed = value;
            if(value) _wasReleased = false;
        }
    }
    private bool _wasReleased;
    public bool wasReleased {
        get { return _wasReleased; }
        set {
            _wasReleased = value;
            if(value) wasPressed = false;
        }
    }

    public VirtualKey(string name, float sensitivity, float gravity, float deadZone) {
        this.name = name;
        this.sensitivity = sensitivity;
        this.gravity = gravity;
        this.deadZone = deadZone;
    }

    public string ChangeName(string name) {
        string oldName = this.name;
        this.name = name;
        return oldName;
    }
}

public class VirtualAxis {
    public string name { get; private set; }
    public VirtualKey positiveKey { get; set; }
    public VirtualKey negativeKey { get; set; }
    public float time { get { return positiveKey.time - (negativeKey != null ? negativeKey.time : 0f); } }
    public float timeRaw { get { return (positiveKey.isHeld ? 1f : 0f) - (negativeKey != null ? (negativeKey.isHeld ? 1f : 0f) : 0f); } }

    public VirtualAxis(string name, string positiveKey, string negativeKey = null) {
        this.name = name;
        this.positiveKey = VirtualKeyManager.LookupVirtualKeyByName(positiveKey);
        this.negativeKey = VirtualKeyManager.LookupVirtualKeyByName(negativeKey);
    }

    public string ChangeName(string name) {
        string oldName = this.name;
        this.name = name;
        return oldName;
    }

    public void ChangeKeys(string positiveKey, string negativeKey = null) {
        this.positiveKey = VirtualKeyManager.LookupVirtualKeyByName(positiveKey);
        this.negativeKey = VirtualKeyManager.LookupVirtualKeyByName(negativeKey);
    }
}
