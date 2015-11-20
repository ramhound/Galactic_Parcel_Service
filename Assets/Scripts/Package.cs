using UnityEngine;
using System.Collections;

public class Package {
    public delegate void OnConditionChange(float condition, float startingCondition);

    public OnConditionChange onConditionChange;
    //public int cargoSize { get; set; }
    public float fragility { get; set; }
    public float startingConditon { get; private set; }
    private float _condition;
    public float condition {
        get { return _condition; }
        set {
            _condition = Mathf.Clamp(value, 0f, startingConditon);
            if(onConditionChange != null) onConditionChange(condition, startingConditon);
        }
    }

    public Package(float fragility, float condition) {
        //this.cargoSize = cargoSize;
        this.fragility = fragility;
        startingConditon = condition;
        _condition = condition;
    }
}
