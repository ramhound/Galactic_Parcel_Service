using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[Flags]
public enum CharacterStyle {
    [EnumDescription("British")]
    British =  (1 << 0),
    [EnumDescription("Desert")]
    Desert =   (1 << 1),
    [EnumDescription("Farm")]
    Farm =     (1 << 2),
    [EnumDescription("Gas")]
    Gas =      (1 << 3),
    [EnumDescription("Ice")]
    Ice =      (1 << 4),
    [EnumDescription("Jungle")]
    Jungle =   (1 << 5),
    [EnumDescription("Rich")]
    Rich =     (1 << 6),
    [EnumDescription("Scottish")]
    Scottish = (1 << 7),
    [EnumDescription("Urban")]
    Urban =    (1 << 8),
    [EnumDescription("Water")]
    Water =    (1 << 9)
}

public class ClientManager : MonoBehaviour {
    private static ClientManager instance;
    public Sprite _farnsberg;
    public CharacterStyleSprites[] characterSprites;
    public static Client farnsberg;
    public static Dictionary<CharacterStyle, Sprite[]> characters = new Dictionary<CharacterStyle, Sprite[]>();
    public static Dictionary<string, List<Client>> allClients = new Dictionary<string, List<Client>>();

    [System.Serializable]
    public struct CharacterStyleSprites {
        public CharacterStyle style;
        public Sprite[] sprites;
    }

    private void Awake() {
        instance = this;
        farnsberg = new Client();
        farnsberg.profilePic = _farnsberg;

        foreach(var cs in characterSprites) {
            var style = cs.style;
            characters[style] = cs.sprites;
        }
    }

    public static Client GenerateClient(CharacterStyle style = 0) {
        int temp = 0;
        if((int)style == 0) {
            foreach(var s in Enum.GetValues(typeof(CharacterStyle)) as int[])
                temp |= s;
            style = (CharacterStyle)temp;
        }

        var styles = style.GetStyles();

        //testing
        var randStyle = styles[UnityEngine.Random.Range(0, styles.Length)];
        var randSprite = characters[randStyle][UnityEngine.Random.Range(0, characters[randStyle].Length)];

        var client = new Client();
        client.profilePic = randSprite;

        return client;
    }
}

public static class CharacterStyleExtensions {
    const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;

    public static string GetDescription(this Enum prop) {
        Type type = prop.GetType();

        FieldInfo info = type.GetField(prop.ToString());
        object[] pas = info.GetCustomAttributes(typeof(EnumDescription), false);
        if(pas.Length > 0)
            return (EnumDescription)pas[0] == null ? prop.ToString() : ((EnumDescription)pas[0]).displayName;
        return prop.ToString();
    }

    public static CharacterStyle[] GetStyles(this CharacterStyle style) {
        var styles = new List<CharacterStyle>((CharacterStyle[])Enum.GetValues(typeof(CharacterStyle)));
        for(int i = styles.Count - 1; i >= 0; i--) {
            if(!style.Contains(styles[i])) styles.Remove(styles[i]);
        }
        return styles.ToArray();
    }
    public static bool Contains(this CharacterStyle main, CharacterStyle prop) {
        return (main & prop) == prop;
    }
}

