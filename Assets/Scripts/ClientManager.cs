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
    public static Dictionary<string, Client> namedClients = new Dictionary<string, Client>();

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

    public static Client GetClient(string name) {
        //will eventually be able to get a preexidting client by name
        return null;
    }

    public static Client GenerateClient(Vector2 index) {
        if(index.x == -1) return ClientManager.farnsberg;
        var client = new Client();
        client.profilePic = characters[(CharacterStyle)(index.x)][(int)index.y];
        client.profilePicIndex = index;

        return client;
    }

    public static Client GenerateClient(Location loc) {
        int temp = 0;
        var style = loc.clientStyles;
        if((int)style == 0) {
            foreach(var s in Enum.GetValues(typeof(CharacterStyle)) as int[])
                temp |= s;
            style = (CharacterStyle)temp;
        }

        var styles = style.GetStyles();

        //testing
        int i1 = UnityEngine.Random.Range(0, styles.Length - 1);
        var randStyle = styles[i1];
        int i2 = UnityEngine.Random.Range(0, characters[randStyle].Length - 1);
        var randSprite = characters[randStyle][i2];

        var client = new Client();
        client.profilePic = randSprite;
        client.profilePicIndex = new Vector2((int)randStyle, i2);
        client.location = loc;

        return client;
    }
}

public static class CharacterStyleExtensions {
    const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;

    public static string GetDescription(this Enum prop) {
        Type type = prop.GetType();

        FieldInfo info = type.GetField(prop.ToString());
        var pas = (object[])(info.GetCustomAttributes(typeof(EnumDescription), false));
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

