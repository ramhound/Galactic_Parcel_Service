using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
// System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Flattens a multidimensional array of type T into a single array
/// </summary>
public static partial class CSharpExtensionMethods {
    #region array extensions
    public static T[] Flatten<T>(this T[,] array) {
        if(array == null) return null;
        T[] flat = new T[array.GetLength(0) * array.GetLength(1)];
        int index = 0;
        for(int i = 0; i < array.GetLength(0); i++) {
            for(int j = 0; j < array.GetLength(1); j++) {
                flat[index++] = array[i, j];
            }
        } return flat;
    }

    // create a subset from a range of indices
    public static T[] RangeSubset<T>(this T[] array, int startIndex, int length) {
        T[] subset = new T[length];
        Array.Copy(array, startIndex, subset, 0, length);
        return subset;
    }

    // create a subset from a specific list of indices
    public static T[] Subset<T>(this T[] array, params int[] indices) {
        T[] subset = new T[indices.Length];
        for(int i = 0; i < indices.Length; i++) {
            subset[i] = array[indices[i]];
        }
        return subset;
    }

    /// <summary>
    /// Converts a single dimensional array into a List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns>The list<T></returns>
    public static List<T> ToList<T>(this T[] array) {
        return new List<T>(array);
    }

    /// <summary>
    /// Returns a count of all non-null entries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns>count of non-null entries</returns>
    public static int NonNullEntriesCount<T>(this T[] array) {
        if(array == null) return 0;
        int index = 0;
        for(int i = 0; i < array.Length; i++) {
            if(array[i] != null) index++;
        } return index;
    }

    public static T[] RemoveNullEntries<T>(this T[] array) {
        List<T> list = new List<T>();
        for(int i = 0; i < array.Length; i++) {
            if(array[i] != null) {
                list.Add(array[i]);
            }
        }
        return list.ToArray();
    }

    #endregion

    #region string extensions
    /// <summary>
    /// Checks to see if two strings are equal regardless of case
    /// </summary>
    /// <param name="str"></param>
    /// <param name="comp"></param>
    /// <returns>If the strings are equal</returns>
    public static bool EqualsIgnoreCase(this string str, string comp) {
        return String.Equals(str, comp.ToLower(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Capitalizes the first letter and return the result
    /// </summary>
    /// <param name="str"></param>
    /// <returns>The resulting string</returns>
    public static string CapitalizeFirst(this string str) {
        if(string.IsNullOrEmpty(str)) {
            return string.Empty;
        }

        char[] strChars = str.ToCharArray();
        strChars[0] = char.ToUpper(strChars[0]);
        return new string(strChars);
    }
    #endregion

    #region object extensions

//#if !UNITY_WP8
//    /// <summary>
//    /// Perform a deep Copy of the object.
//    /// </summary>
//    /// <typeparam name="T">The type of object being copied.</typeparam>
//    /// <param name="source">The object instance to copy.</param>
//    /// <returns>The copied object</returns>
//    public static T Clone<T>(this T source) {
//        if(!typeof(T).IsSerializable) {
//            throw new ArgumentException("The type must be serializable.", "source");
//        }

//        // Don't serialize a null object, simply return the default for that object
//        if(object.ReferenceEquals(source, null)) {
//            return default(T);
//        }

//        IFormatter formatter = new BinaryFormatter();
//        Stream stream = new MemoryStream();
//        using(stream) {
//            formatter.Serialize(stream, source);
//            stream.Seek(0, SeekOrigin.Begin);
//            return (T)formatter.Deserialize(stream);
//        }
//    }
//#endif

#endregion
}
