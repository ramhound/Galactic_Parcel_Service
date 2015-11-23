using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class EnumExtensions {
    const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;

    public static string GetDescription(this Enum prop) {
        Type type = prop.GetType();

        FieldInfo info = type.GetField(prop.ToString());
        var pas = (object[])(info.GetCustomAttributes(typeof(EnumDescription), false));
        if(pas.Length > 0)
            return (EnumDescription)pas[0] == null ? prop.ToString() : ((EnumDescription)pas[0]).displayName;
        return prop.ToString();
    }
}
    public class EnumDescription : System.Attribute {
        public string displayName { get; private set; }

        public EnumDescription(string name) {
            displayName = name;
        }
}

public class BitMaskAttribute : PropertyAttribute {
    public System.Type propType;
    public BitMaskAttribute(System.Type aType) {
        propType = aType;
    }
}
