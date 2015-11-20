using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class GameObjectExtensions {
     /// <summary>
    /// Returns the first occurrence of a child with the attached Component (excludes grandchildren)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T Find<T>(this GameObject gameObject) where T : Component {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
            if(type != null)
                return type;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the attached Component (excludes grandchildren)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T FindChildByType<T>(this GameObject gameObject) where T : Component {
        return gameObject.transform.Find<T>();
    }

    /// <summary>
    /// Returns all children with the attached Component (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByType<T>(this GameObject gameObject) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>(); 
            if(type != null)
                children.Add(type);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the attached Component (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindRecursively<T>(this GameObject gameObject) where T : Component {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
            if(type != null)
                return type;
        }

        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T returnedType = gameObject.transform.GetChild(i).FindRecursively<T>();
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the attached Component (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByTypeRecursively<T>(this GameObject gameObject) where T : Component {
        return gameObject.transform.FindRecursively<T>();
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy with the attached Component 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByTypeRecursively<T>(this GameObject gameObject) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(gameObject.transform.FindChildrenByType<T>()));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<T>(gameObject.transform.FindChildrenByTypeRecursively<T>()));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindByTag(this GameObject gameObject, string tag) {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(tag))
                return gameObject.transform.GetChild(i).gameObject;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindChildByTag(this GameObject gameObject, string tag) {
        return gameObject.FindByTag(tag);
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByTag<T>(this GameObject gameObject, string tag) where T : Component {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(tag)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type == null)
                    return type; 
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByTag<T>(this GameObject gameObject, string tag) where T : Component {
        return gameObject.FindByTag<T>(tag);
    }

    /// <summary>
    /// Returns all children with the given tag (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByTag(this GameObject gameObject, string tag) {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(tag))
                children.Add(gameObject.transform.GetChild(i).gameObject);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children with the given tag and type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByTag<T>(this GameObject gameObject, string tag) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(tag)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindByTagRecursively(this GameObject gameObject, string tag) {
        //first checks to see if any immediate children have the tag
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(tag))
                return gameObject.transform.GetChild(i).gameObject;
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            Transform returnedGameObject = gameObject.transform.GetChild(i).FindByTagRecursively(tag);
            if(returnedGameObject != null)
                return returnedGameObject.gameObject;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByTagRecursively<T>(this GameObject gameObject, string tag) where T : Component {
        //first checks to see if any immediate children have the tag
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(tag)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T returnedType = gameObject.transform.GetChild(i).FindByTagRecursively<T>(tag);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindChildByTagRecusively(this GameObject gameObject, string tag) {
        Transform t = gameObject.transform.FindByTagRecursively(tag);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByTagRecusively<T>(this GameObject gameObject, string tag) where T : Component {
        return gameObject.transform.FindByTagRecursively<T>(tag);
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy by the given tag
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByTagRecursively(this GameObject gameObject, string tag) {
        List<GameObject> children = new List<GameObject>();
        children.AddRange(new List<GameObject>(gameObject.FindChildrenByTag(tag)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<GameObject>(gameObject.FindChildrenByTagRecursively(tag)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy by the given tag and Type
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByTagRecursively<T>(this GameObject gameObject, string tag) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(gameObject.transform.FindChildrenByTag<T>(tag)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<T>(gameObject.transform.FindChildrenByTagRecursively<T>(tag)));
        } return children.ToArray();
    }

    /// <summary>
    /// Wrapper function for GameObject.Find("name");
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject FindByName(this GameObject gameObject, string name) {
        Transform t = gameObject.transform.Find(name);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given name and type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindByName<T>(this GameObject gameObject, string name) where T : Component {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Equals(name)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Wrapper function for GameObject.Find("name");
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject FindChildByName(this GameObject gameObject, string name) {
        Transform t = gameObject.transform.Find(name);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given name and type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindChildByName<T>(this GameObject gameObject, string name) where T : Component {
        return gameObject.transform.FindByName<T>(name);
    }

    /// <summary>
    /// Returns all children with the supplied name (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByName(this GameObject gameObject, string name) {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Equals(name))
                children.Add(gameObject.transform.GetChild(i).gameObject);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children with the supplied name and type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByName<T>(this GameObject gameObject, string name) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Equals(name)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the supplied name (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindByNameRecursively(this GameObject gameObject, string name) {
        //first checks to see if any immediate children have the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Equals(name))
                return gameObject.transform.GetChild(i).gameObject;
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            Transform returnedGameObject = gameObject.transform.GetChild(i).FindByNameRecursively(name);
            if(returnedGameObject != null)
                return returnedGameObject.gameObject;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the supplied name and type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByNameRecursively<T>(this GameObject gameObject, string name) where T : Component {
        //first checks to see if any immediate children have the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Equals(name)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T returnedGameObject = gameObject.transform.GetChild(i).FindByNameRecursively<T>(name);
            if(returnedGameObject != null)
                return returnedGameObject;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name is equal to the supplied name (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindChildByNameRecursively(this GameObject gameObject, string name) {
        Transform t = gameObject.transform.FindByNameRecursively(name);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the supplied name and type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByNameRecursively<T>(this GameObject gameObject, string name) where T : Component {
        return gameObject.transform.FindByNameRecursively<T>(name);
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy with the supplied name
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByNameRecursively(this GameObject gameObject, string name) {
        List<GameObject> children = new List<GameObject>();
        children.AddRange(new List<GameObject>(gameObject.FindChildrenByName(name)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<GameObject>(gameObject.FindChildrenByNameRecursively(name)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy with the supplied name and type
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByNameRecursively<T>(this GameObject gameObject, string name) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(gameObject.transform.FindChildrenByName<T>(name)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<T>(gameObject.transform.FindChildrenByNameRecursively<T>(name)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject FindByBeginningOfName(this GameObject gameObject, string part) {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.StartsWith(part))
                return child;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindByBeginningOfName<T>(this GameObject gameObject, string part) where T : Component {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.StartsWith(part)) {
                T type = child.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject FindChildByBeginningOfname(this GameObject gameObject, string part) {
        Transform t = gameObject.transform.FindByBeginningOfName(part);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindChildByBeginningOfname<T>(this GameObject gameObject, string part) where T : Component {
        return gameObject.transform.FindByBeginningOfName<T>(part);
    }

    /// <summary>
    /// Returns all children who's name starts with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByBeginningOfName(this GameObject gameObject, string part) {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.StartsWith(part))
                children.Add(child);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children who's name starts with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByBeginningOfName<T>(this GameObject gameObject, string part) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.StartsWith(part)) {
                T type = child.gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child thats starts with the part (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindByBeginningOfNameRecursively(this GameObject gameObject, string part) {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.StartsWith(part))
                return gameObject.transform.GetChild(i).gameObject;
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            Transform returnedGameObject = gameObject.transform.GetChild(i).FindByBeginningOfNameRecursively(part);
            if(returnedGameObject != null)
                return returnedGameObject.gameObject;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child thats starts with the part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByBeginningOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.StartsWith(part)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T returnedType = gameObject.transform.GetChild(i).FindByBeginningOfNameRecursively<T>(part);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child thats starts with the part (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindChildByBeginningOfNameRecusively(this GameObject gameObject, string part) {
        Transform t = gameObject.transform.FindByBeginningOfNameRecursively(part);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child thats starts with the part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByBeginningOfNameRecusively<T>(this GameObject gameObject, string part) where T : Component {
        return gameObject.transform.FindByBeginningOfNameRecursively<T>(part);
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy who's name starts with the given part
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByBeginningOfNameRecursively(this GameObject gameObject, string part) {
        List<GameObject> children = new List<GameObject>();
        children.AddRange(new List<GameObject>(gameObject.FindChildrenByBeginningOfName(part)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<GameObject>(gameObject.FindChildrenByNameRecursively(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy who's name starts with the given part and is of type
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByBeginningOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(gameObject.transform.FindChildrenByBeginningOfName<T>(part)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<T>(gameObject.transform.FindChildrenByNameRecursively<T>(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject FindByEndOfName(this GameObject gameObject, string part) {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.EndsWith(part))
                return child;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindByEndOfName<T>(this GameObject gameObject, string part) where T : Component {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.EndsWith(part)) {
                T type = child.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject FindChildByEndOfName(this GameObject gameObject, string part) {
        Transform t = gameObject.transform.FindByEndOfName(part);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindChildByEndOfName<T>(this GameObject gameObject, string part) where T : Component {
        return gameObject.transform.FindByEndOfName<T>(part);
    }

    /// <summary>
    /// Returns all children who's name ends with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByEndOfName(this GameObject gameObject, string part) {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.EndsWith(part))
                children.Add(gameObject.transform.GetChild(i).gameObject);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children who's name ends with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByEndOfName<T>(this GameObject gameObject, string part) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.EndsWith(part)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindByEndOfNameRecursively(this GameObject gameObject, string part) {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.EndsWith(part))
                return gameObject.transform.GetChild(i).gameObject;
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            Transform returnedGameObject = gameObject.transform.GetChild(i).FindByEndOfNameRecursively(part);
            if(returnedGameObject != null)
                return returnedGameObject.gameObject;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type(includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByEndOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.EndsWith(part)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T returnedType = gameObject.transform.GetChild(i).FindByEndOfNameRecursively<T>(part);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindChildByEndOfNameRecursively(this GameObject gameObject, string part) {
        Transform t = gameObject.transform.FindByEndOfNameRecursively(part);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByEndOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        return gameObject.transform.FindByEndOfNameRecursively<T>(part);
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy who's name ends with the given part
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByEndOfNameRecursively(this GameObject gameObject, string part) {
        List<GameObject> children = new List<GameObject>();
        children.AddRange(new List<GameObject>(gameObject.FindChildrenByEndOfName(part)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<GameObject>(gameObject.FindChildrenByEndOfNameRecursively(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy who's name ends with the given part and is of type
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByEndOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(gameObject.transform.FindChildrenByEndOfName<T>(part)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<T>(gameObject.transform.FindChildrenByEndOfNameRecursively<T>(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject FindByPartOfName(this GameObject gameObject, string part) {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.Contains(part))
                return child;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindByPartOfName<T>(this GameObject gameObject, string part) where T : Component {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if(child.name.Contains(part)) {
                T type = child.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject FindChildByPartOfName(this GameObject gameObject, string part) {
        Transform t = gameObject.transform.FindByPartOfName(part);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindChildByPartOfName<T>(this GameObject gameObject, string part) where T : Component {
        return gameObject.transform.FindByPartOfName<T>(part);
    }

    /// <summary>
    /// Returns all children who's name contains the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByPartOfName(this GameObject gameObject, string part) {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Contains(part))
                children.Add(gameObject.transform.GetChild(i).gameObject);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children who's name contains the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByPartOfName<T>(this GameObject gameObject, string part) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Contains(part)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindByPartOfNameRecursively(this GameObject gameObject, string part) {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Contains(part))
                return gameObject.transform.GetChild(i).gameObject;
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            Transform returnedGameObject = gameObject.transform.GetChild(i).FindByPartOfNameRecursively(part);
            if(returnedGameObject != null)
                return returnedGameObject.gameObject;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByPartOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).name.Contains(part)) {
                T type = gameObject.transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            T returnedType = gameObject.transform.GetChild(i).FindByPartOfNameRecursively<T>(part);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindChildBypartOfNameRecursively(this GameObject gameObject, string part) {
        Transform t = gameObject.transform.FindByPartOfNameRecursively(part);
        if(t != null) return t.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildBypartOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        return gameObject.transform.FindByPartOfNameRecursively<T>(part);
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy who's name contains the supplied part
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject[] FindChildrenByPartOfNameRecursively(this GameObject gameObject, string part) {
        List<GameObject> children = new List<GameObject>();
        children.AddRange(new List<GameObject>(gameObject.FindChildrenByPartOfName(part)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<GameObject>(gameObject.FindChildrenByPartOfNameRecursively(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the GameObject hierarchy who's name contains the supplied part
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByPartOfNameRecursively<T>(this GameObject gameObject, string part) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(gameObject.transform.FindChildrenByPartOfName<T>(part)));
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            children.AddRange(new List<T>(gameObject.transform.FindChildrenByPartOfNameRecursively<T>(part)));
        } return children.ToArray();
    }
}
