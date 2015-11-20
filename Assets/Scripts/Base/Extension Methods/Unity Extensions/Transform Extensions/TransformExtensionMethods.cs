using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class TransformExtensions {
    public static T Find<T>(this Transform transform) where T :Component {
        return transform.FindByType<T>();
    }

    public static T Find<T>(this Transform transform, string name) where T : Component {
        return transform.FindByName<T>(name);
    }

    /// <summary>
    /// Returns the first occurrence of a child with the attached Component (excludes grandchildren)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static T FindByType<T>(this Transform transform) where T : Component {
        for(int i = 0; i < transform.childCount; i++) {
            T type = transform.GetChild(i).gameObject.GetComponent<T>();
            if(type != null)
                return type;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the attached Component (excludes grandchildren)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static T FindChildByType<T>(this Transform transform) where T : Component {
        return transform.Find<T>();
    }

    /// <summary>
    /// Returns all children with the attached Component (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByType<T>(this Transform transform) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < transform.childCount; i++) {
            T type = transform.GetChild(i).gameObject.GetComponent<T>();
            if(type != null)
                children.Add(type);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the attached Component (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindRecursively<T>(this Transform transform) where T : Component {
        for(int i = 0; i < transform.childCount; i++) {
            T type = transform.GetChild(i).gameObject.GetComponent<T>();
            if(type != null)
                return type;
        }

        for(int i = 0; i < transform.childCount; i++) {
            T returnedType = transform.GetChild(i).FindRecursively<T>();
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the attached Component (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByTypeRecursively<T>(this Transform transform) where T : Component {
        return transform.FindRecursively<T>();
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy with the attached Component 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByTypeRecursively<T>(this Transform transform) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(transform.FindChildrenByType<T>()));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<T>(transform.FindChildrenByTypeRecursively<T>()));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindByTag(this Transform transform, string tag) {
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).CompareTag(tag))
                return transform.GetChild(i);
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindChildByTag(this Transform transform, string tag) {
        return transform.FindByTag(tag);
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByTag<T>(this Transform transform, string tag) where T : Component {
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).CompareTag(tag)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type == null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByTag<T>(this Transform transform, string tag) where T : Component {
        return FindByTag<T>(transform, tag);
    }

    /// <summary>
    /// Returns all children with the given tag (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByTag(this Transform transform, string tag) {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).CompareTag(tag))
                children.Add(transform.GetChild(i));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children with the given tag and type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByTag<T>(this Transform transform, string tag) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).CompareTag(tag)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindByTagRecursively(this Transform transform, string tag) {
        //first checks to see if any immediate children have the tag
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).CompareTag(tag))
                return transform.GetChild(i);
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            Transform returnedTransform = transform.GetChild(i).FindByTagRecursively(tag);
            if(returnedTransform != null)
                return returnedTransform;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByTagRecursively<T>(this Transform transform, string tag) where T : Component {
        //first checks to see if any immediate children have the tag
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).CompareTag(tag)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            T returnedType = transform.GetChild(i).FindByTagRecursively<T>(tag);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindChildByTagRecusively(this Transform transform, string tag) {
        return transform.FindByTagRecursively(tag);
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given tag and type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByTagRecusively<T>(this Transform transform, string tag) where T : Component {
        return transform.FindByTagRecursively<T>(tag);
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy by the given tag
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByTagRecursively(this Transform transform, string tag) {
        List<Transform> children = new List<Transform>();
        children.AddRange(new List<Transform>(transform.FindChildrenByTag(tag)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<Transform>(transform.FindChildrenByTagRecursively(tag)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy by the given tag and Type
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByTagRecursively<T>(this Transform transform, string tag) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(transform.FindChildrenByTag<T>(tag)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<T>(transform.FindChildrenByTagRecursively<T>(tag)));
        } return children.ToArray();
    }

    /// <summary>
    /// Wrapper function for Transform.Find("name");
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindByName(this Transform transform, string name) {
        return transform.Find(name);
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given name and type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindByName<T>(this Transform transform, string name) where T : Component {
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Equals(name)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Wrapper function for Transform.Find("name");
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChildByName(this Transform transform, string name) {
        return transform.Find(name);
    }

    /// <summary>
    /// Returns the first occurrence of a child with the given name and type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindChildByName<T>(this Transform transform, string name) where T : Component {
        return transform.FindByName<T>(name);
    }

    /// <summary>
    /// Returns all children with the supplied name (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByName(this Transform transform, string name) {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Equals(name))
                children.Add(transform.GetChild(i));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children with the supplied name and type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByName<T>(this Transform transform, string name) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Equals(name)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child with the supplied name (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindByNameRecursively(this Transform transform, string name) {
        //first checks to see if any immediate children have the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Equals(name))
                return transform.GetChild(i);
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            Transform returnedTransform = transform.GetChild(i).FindByNameRecursively(name);
            if(returnedTransform != null)
                return returnedTransform;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child with the supplied name and type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByNameRecursively<T>(this Transform transform, string name) where T : Component {
        //first checks to see if any immediate children have the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Equals(name)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            T returnedTransform = transform.GetChild(i).FindByNameRecursively<T>(name);
            if(returnedTransform != null)
                return returnedTransform;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name is equal to the supplied name (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindChildByNameRecursively(this Transform transform, string name) {
        return transform.FindByNameRecursively(name);
    }

    /// <summary>
    /// Returns the first occurrence of a child with the supplied name and type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByNameRecursively<T>(this Transform transform, string name) where T : Component {
        return transform.FindByNameRecursively<T>(name);
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy with the supplied name
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByNameRecursively(this Transform transform, string name) {
        List<Transform> children = new List<Transform>();
        children.AddRange(new List<Transform>(transform.FindChildrenByName(name)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<Transform>(transform.FindChildrenByNameRecursively(name)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy with the supplied name and type
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByNameRecursively<T>(this Transform transform, string name) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(transform.FindChildrenByName<T>(name)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<T>(transform.FindChildrenByNameRecursively<T>(name)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform FindByBeginningOfName(this Transform transform, string part) {
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if(child.name.StartsWith(part))
                return child;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindByBeginningOfName<T>(this Transform transform, string part) where T : Component {
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if(child.name.StartsWith(part)) {
                T type = child.gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform FindChildByBeginningOfname(this Transform transform, string part) {
        return transform.FindByBeginningOfName(part);
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name starts with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindChildByBeginningOfname<T>(this Transform transform, string part) where T : Component {
        return transform.FindByBeginningOfName<T>(part);
    }

    /// <summary>
    /// Returns all children who's name starts with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByBeginningOfName(this Transform transform, string part) {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if(child.name.StartsWith(part))
                children.Add(child);
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children who's name starts with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByBeginningOfName<T>(this Transform transform, string part) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
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
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindByBeginningOfNameRecursively(this Transform transform, string part) {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.StartsWith(part))
                return transform.GetChild(i);
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            Transform returnedTransform = transform.GetChild(i).FindByBeginningOfNameRecursively(part);
            if(returnedTransform != null)
                return returnedTransform;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child thats starts with the part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByBeginningOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.StartsWith(part)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            T returnedType = transform.GetChild(i).FindByBeginningOfNameRecursively<T>(part);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child thats starts with the part (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindChildByBeginningOfNameRecusively(this Transform transform, string part) {
        return transform.FindByBeginningOfNameRecursively(part);
    }

    /// <summary>
    /// Returns the first occurrence of a child thats starts with the part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByBeginningOfNameRecusively<T>(this Transform transform, string part) where T : Component {
        return transform.FindByBeginningOfNameRecursively<T>(part);
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy who's name starts with the given part
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByBeginningOfNameRecursively(this Transform transform, string part) {
        List<Transform> children = new List<Transform>();
        children.AddRange(new List<Transform>(transform.FindChildrenByBeginningOfName(part)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<Transform>(transform.FindChildrenByNameRecursively(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy who's name starts with the given part and is of type
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByBeginningOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(transform.FindChildrenByBeginningOfName<T>(part)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<T>(transform.FindChildrenByNameRecursively<T>(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform FindByEndOfName(this Transform transform, string part) {
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if(child.name.EndsWith(part))
                return child;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindByEndOfName<T>(this Transform transform, string part) where T : Component {
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if(child.name.EndsWith(part)) {
                T type = child.gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform FindChildByEndOfName(this Transform transform, string part) {
        return transform.FindByEndOfName(part);
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindChildByEndOfName<T>(this Transform transform, string part) where T : Component {
        return transform.FindByEndOfName<T>(part);
    }

    /// <summary>
    /// Returns all children who's name ends with the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByEndOfName(this Transform transform, string part) {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.EndsWith(part))
                children.Add(transform.GetChild(i));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children who's name ends with the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByEndOfName<T>(this Transform transform, string part) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.EndsWith(part)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindByEndOfNameRecursively(this Transform transform, string part) {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.EndsWith(part))
                return transform.GetChild(i);
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            Transform returnedTransform = transform.GetChild(i).FindByEndOfNameRecursively(part);
            if(returnedTransform != null)
                return returnedTransform;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type(includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByEndOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.EndsWith(part)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            T returnedType = transform.GetChild(i).FindByEndOfNameRecursively<T>(part);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindChildByEndOfNameRecursively(this Transform transform, string part) {
        return transform.FindByEndOfNameRecursively(part);
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name ends with the supplied part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildByEndOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        return transform.FindByEndOfNameRecursively<T>(part);
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy who's name ends with the given part
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByEndOfNameRecursively(this Transform transform, string part) {
        List<Transform> children = new List<Transform>();
        children.AddRange(new List<Transform>(transform.FindChildrenByEndOfName(part)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<Transform>(transform.FindChildrenByEndOfNameRecursively(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy who's name ends with the given part and is of type
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByEndOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(transform.FindChildrenByEndOfName<T>(part)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<T>(transform.FindChildrenByEndOfNameRecursively<T>(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform FindByPartOfName(this Transform transform, string part) {
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if(child.name.Contains(part))
                return child;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindByPartOfName<T>(this Transform transform, string part) where T : Component {
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if(child.name.Contains(part)) {
                T type = child.gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform FindChildByPartOfName(this Transform transform, string part) {
        return transform.FindByPartOfName(part);
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T FindChildByPartOfName<T>(this Transform transform, string part) where T : Component {
        return transform.FindByPartOfName<T>(part);
    }

    /// <summary>
    /// Returns all children who's name contains the supplied part (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByPartOfName(this Transform transform, string part) {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Contains(part))
                children.Add(transform.GetChild(i));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children who's name contains the supplied part and is of type (excludes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public static T[] FindChildrenByPartOfName<T>(this Transform transform, string part) where T : Component {
        List<T> children = new List<T>();
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Contains(part)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    children.Add(type);
            }
        } return children.ToArray();
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindByPartOfNameRecursively(this Transform transform, string part) {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Contains(part))
                return transform.GetChild(i);
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            Transform returnedTransform = transform.GetChild(i).FindByPartOfNameRecursively(part);
            if(returnedTransform != null)
                return returnedTransform;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindByPartOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        //first checks to see if any immediate children start with the name
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).name.Contains(part)) {
                T type = transform.GetChild(i).gameObject.GetComponent<T>();
                if(type != null)
                    return type;
            }
        }
        //search every child recursively
        for(int i = 0; i < transform.childCount; i++) {
            T returnedType = transform.GetChild(i).FindByPartOfNameRecursively<T>(part);
            if(returnedType != null)
                return returnedType;
        } return null;
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform FindChildBypartOfNameRecursively(this Transform transform, string part) {
        return transform.FindByPartOfNameRecursively(part);
    }

    /// <summary>
    /// Returns the first occurrence of a child who's name contains the supplied part and is of type (includes grandchildren)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T FindChildBypartOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        return transform.FindByPartOfNameRecursively<T>(part);
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy who's name contains the supplied part
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform[] FindChildrenByPartOfNameRecursively(this Transform transform, string part) {
        List<Transform> children = new List<Transform>();
        children.AddRange(new List<Transform>(transform.FindChildrenByPartOfName(part)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<Transform>(transform.FindChildrenByPartOfNameRecursively(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Returns all children in the Transform hierarchy who's name contains the supplied part
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static T[] FindChildrenByPartOfNameRecursively<T>(this Transform transform, string part) where T : Component {
        List<T> children = new List<T>();
        children.AddRange(new List<T>(transform.FindChildrenByPartOfName<T>(part)));
        for(int i = 0; i < transform.childCount; i++) {
            children.AddRange(new List<T>(transform.FindChildrenByPartOfNameRecursively<T>(part)));
        } return children.ToArray();
    }

    /// <summary>
    /// Sets the given tag on every child of this Transform (excludes grandchildren)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform SetTagOnAllChildren(this Transform parent, string tag) {
        for(int i = 0; i < parent.childCount; i++) {
            parent.GetChild(i).tag = tag;
        } return parent;
    }

    /// <summary>
    /// Sets the given tag on every child of this Transform (includes grandchildren)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Transform SetTagOnAllChildrenRecursively(this Transform parent, string tag) {
        for(int i = 0; i < parent.childCount; i++) {
            parent.GetChild(i).tag = tag;
            parent.GetChild(i).SetTagOnAllChildrenRecursively(tag);
        } return parent;
    }
}
