using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Exception handler for the SOS class
/// </summary>
public class PixelDogExcpetion : Exception {
    public PixelDogExcpetion() : base() { }
    public PixelDogExcpetion(string reason) : base(reason) { }
}
/// <summary>
/// This class is a collection of static functions designed to help with everyday use
/// </summary>
public class PixelDog : MonoBehaviour {
    private static PixelDog instance;
    private static Transform _prototype;  //Transform to hold the prototypes
    private static Dictionary<string, Prototype> hierarchy;

    public string _version = "1.0.0.0";
    public static string version;
    public string _updateURL = "http://www.sosmediadesigns.net/patches/";
    public static string updateURL;
    public KeyCode _quitKey = KeyCode.F12;
    public static KeyCode quitKey;
    public bool preventScreenSleep = false;
    public bool _isPaidVersion = true;
    public static bool isPaidVersion;
    public static bool isMobile;
    public Command[] commandListeners;

    private void OnEnable() {}
    private void Start() {}
    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        CheckIfMobile();
        CopyVarsToStatic();

        if(preventScreenSleep && isMobile) Screen.sleepTimeout = SleepTimeout.NeverSleep;

        CreatePrototypesObject();
        HidePrototypes();
    }

    private void CopyVarsToStatic() {
        version = _version;
        quitKey = _quitKey;
        isPaidVersion = _isPaidVersion;
        updateURL = _updateURL;
    }


    private void CreatePrototypesObject() {
        _prototype = transform.Find("Prototypes") ?? new GameObject("Prototypes").transform;
        _prototype.parent = transform;
        hierarchy = Prototype.CreatePrototypeHierarchy(_prototype);
    }

    private void HidePrototypes() {
        for(int i = 0; i < _prototype.childCount; i++)
            _prototype.GetChild(i).gameObject.SetActive(false);
    }

    private void Update() {
        if(Input.GetKeyUp(quitKey))
            Application.Quit();

#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyUp(KeyCode.F5)) {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.LogWarning("Playerprefs have been deleted!");
        }
#endif
    }

    public void OnLevelWasLoaded() {
        if(instance != null && ReferenceEquals(instance, this)) {
            CreatePrototypesObject();
            HidePrototypes();
        }
    }

    #region prototype handling
    /// <summary>
    /// Returns the Prototypes object in the scene
    /// </summary>
    public static Transform prototype { get { return _prototype; } }

    /// <summary>
    /// Finds the requested prototype in the prototypes object and activates it in the scene
    /// </summary>
    /// <param name="name"></param>
    /// <param name="deepSearch"></param>
    /// <returns></returns>
    private static T[] GetPrototypeObjects<T>(string name, int count) where T : UnityEngine.Object {
        name = name.ToLower();
        char[] seperator = new char[] { '/' };
        string[] parentNames = name.Split(seperator, StringSplitOptions.None);
        T[] prototypes = new T[count];
        Prototype prototypeObject;
        if(hierarchy.TryGetValue(parentNames[0], out prototypeObject)) {
            for(int i = 1; i < parentNames.Length; i++) {
                prototypeObject = prototypeObject.GetChild(parentNames[i]);
                if(prototypeObject == null) {
                    Debug.LogError("No prototype found! Check if the prototype exists or the name is spelled correctly");
                    return null;
                }
            }
        } else {
            if(prototypeObject == null) {
                Debug.LogError("No prototype by that name found!");
                return null;
            }
        }

        for(int i = 0; i < count; i++)
            prototypes[i] = Instantiate(prototypeObject.gameObject) as T;
        return prototypes;
    }

    public static GameObject CreateObject(string name, Vector3? position = null, bool isActive = true) {
        GameObject go = PreFabricate(name, isActive: isActive)[0];
        if(go != null && position.HasValue) go.transform.position = position.Value;
        return go;
    }

    public static T CreateObject<T>(string name, Vector3? position = null, bool isActive = true) where T : Component {
        T go = PreFabricate<T>(name, isActive: isActive)[0];
        if(go != null && position.HasValue) go.transform.position = position.Value;
        return go;
    }

    /// <summary>
    /// Creates a specified number of prototype objects in the scene
    /// </summary>
    /// <param name="name"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static GameObject[] PreFabricate(string name, int count = 1, bool isActive = true) {
        GameObject[] gos = GetPrototypeObjects<GameObject>(name, count);
        for(int i = 0; i < gos.Length; i++)
            gos[i].SetActive(isActive);
        return gos;
    }

    public static T[] PreFabricate<T>(string name, int count = 1, bool isActive = true) where T : Component {
        T[] gos = GetPrototypeObjects<T>(name, count);
        for(int i = 0; i < gos.Length; i++)
            gos[i].gameObject.SetActive(isActive);
        return gos;
    }

    public static void AddPrototype(params GameObject[] gos) {
        for(int i = 0; i < gos.Length; i++) {
            gos[i].transform.parent = _prototype;
        }

        hierarchy = Prototype.CreatePrototypeHierarchy(_prototype);
    }

    /// <summary>
    /// <para>Wrapper for Unity native Destroy(GameObject)</para>
    /// <para>(I eventually want destroyed objects to go into a pool)</para>
    /// </summary>
    /// <param name="obj"></param>
    public static void DestroyObject(GameObject obj) {
        //reset all scripts attached somehow and add it to a pool for reuse later on
        Destroy(obj);//destroy for now
    }

    #endregion

    #region delegate execution
    /// <summary>
    /// Call the passed function in x time, optional callBack
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="method"></param>
    /// <param name="callBack"></param>
    public static void ExecuteMethod(float delay, Action method, Action callBack = null) {
        instance.StartCoroutine(instance._ExecuteMethod(delay, method, callBack));
    }

    /// <summary>
    /// Internal execution of the delegates
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="method"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    private IEnumerator _ExecuteMethod(float delay, Action method, Action callBack) {
        yield return new WaitForSeconds(delay);
        method();
        if(callBack != null) callBack();
    }
    #endregion

    #region command handling
    public static void ExecuteCommand(string commandSent) {
        string[] args = commandSent.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        for(int i = 0; i < instance.commandListeners.Length; i++) {
            Command cmd = instance.commandListeners[i];
            for(int j = 0; j < cmd.commands.Length; j++) {
                if(cmd.commands[j].EqualsIgnoreCase(args[0])) {
                    if(cmd.component == null) {
                        Debug.LogError("Listener Component can not be empty!");
                        return;
                    }
                    ICommandlistener listener = (ICommandlistener)cmd.component.GetComponent(typeof(ICommandlistener));
                    if(listener == null) {
                        Debug.LogError("Component must implement ICommandListener");
                        return;
                    }
                    if(!listener.OnCommand(cmd.listenerName, args[0], args.RangeSubset(1, args.Length - 1)))
                        Debug.LogWarning("Command did not complete correctly");
                }
            }
        }
    }
    #endregion

    private static void CheckIfMobile() {
        if(SystemInfo.supportsAccelerometer) isMobile = true; //works in MOST casses
    }

    public static bool IsPointOnScreen(Vector2 point) {
        return (point.x >= 0 && point.x <= Screen.width) && (point.y >= 0 && point.y <= Screen.height);
    }
}

[System.Serializable]
public class Command {
    public string listenerName;
    public Component component;
    public string[] commands;
}

public interface ICommandlistener {
    bool OnCommand(string commandName, string command, string[] args);
}

public class Prototype {
    public Dictionary<string, Prototype> hierarchy;
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }

    public Prototype(GameObject go) {
        gameObject = go;
        transform = go.transform;
        SetChildrenHierarchy();
    }

    private void SetChildrenHierarchy() {
        if(transform.childCount > 0) {
            hierarchy = new Dictionary<string, Prototype>();
            for(int i = 0; i < transform.childCount; i++) {
                hierarchy.Add(transform.GetChild(i).name.ToLower(), new Prototype(transform.GetChild(i).gameObject));
            }
        }
    }

    public Prototype GetChild(string key) {
        Prototype prototypeObject = null;
        if(hierarchy != null)
            hierarchy.TryGetValue(key, out prototypeObject);
        return prototypeObject;
    }

    public static Dictionary<string, Prototype> CreatePrototypeHierarchy(Transform root) {
        Dictionary<string, Prototype> hierarchy = new Dictionary<string, Prototype>();
        for(int i = 0; i < root.childCount; i++)
            hierarchy.Add(root.GetChild(i).name.ToLower(), new Prototype(root.GetChild(i).gameObject));
        return hierarchy;
    }


}