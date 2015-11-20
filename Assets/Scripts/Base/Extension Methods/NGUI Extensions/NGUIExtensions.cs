using UnityEngine;
using System.Collections;

public static partial class NGUIExtensions {
    public static string GetSpriteNameByEndOfName(this UIAtlas atlas, string part) {
        for(int i = 0; i < atlas.GetListOfSprites().size; i++) {
            if(atlas.GetListOfSprites()[i].EndsWith(part))
                return atlas.GetListOfSprites()[i];
        } return null;
    }
}
