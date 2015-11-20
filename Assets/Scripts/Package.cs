using UnityEngine;
using System.Collections;

public class Package {
    public delegate void OnConditionChange(float condition, float startingCondition);

    public Client sender;
    public Client receiver;
    public Vector2 size;
    public float fragility;
}
