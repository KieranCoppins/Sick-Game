using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUIHandler : MonoBehaviour
{
    [SerializeField] private Canvas _debugCanvas;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _currentAction;
    [SerializeField] private TMP_Text _actionQueue;
    [SerializeField] private TMP_Text _hasLOS;

    private BaseMob _mob;

    void Start()
    {
        _mob = GetComponent<BaseMob>();
        _name.text = gameObject.name;
        _debugCanvas.enabled = (_mob.DebugFlags & DebugFlags.DecisionTree) == DebugFlags.DecisionTree;
    }

    void FixedUpdate()
    {
        if (!((_mob.DebugFlags & DebugFlags.DecisionTree) == DebugFlags.DecisionTree))
            return;
        if (_mob.Target != null)
            _hasLOS.text = "LoS: " + _mob.HasLineOfSight(_mob.Target.position);
        else
            _hasLOS.text = "LoS: null";
    }
}
