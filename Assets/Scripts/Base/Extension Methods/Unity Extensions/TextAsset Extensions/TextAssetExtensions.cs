using UnityEngine;
using System;
using System.Text.RegularExpressions;

public static partial class TextAssetExtensions {
    public static string RemoveComments(this TextAsset textAsset) {
        string blockComments = @"/\*(.*?)\*/";
        string lineComments = @"//(.*?)\r?\n";

        return Regex.Replace(textAsset.text,
            blockComments + "|" + lineComments,
            match => {
                if(match.Value.StartsWith("/*") || match.Value.StartsWith("//"))
                    return match.Value.StartsWith("//") ? Environment.NewLine : "";
                return match.Value;
            }, RegexOptions.Singleline);
    }
}
