using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUIHandler : MonoBehaviour
{
    [SerializeField] Canvas debugCanvas;
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text currentAction;
    [SerializeField] TMP_Text actionQueue;
    [SerializeField] TMP_Text hasLOS;

    BaseMob mob;

    void Start()
    {
        mob = GetComponent<BaseMob>();
        Name.text = gameObject.name;
        debugCanvas.enabled = (mob.debugFlags & DebugFlags.DecisionTree) == DebugFlags.DecisionTree;
    }

    void FixedUpdate()
    {
        if (!((mob.debugFlags & DebugFlags.DecisionTree) == DebugFlags.DecisionTree))
            return;

        currentAction.text = "Current Action(s): " + mob.GetCurrentActionText();
        actionQueue.text = "Action Queue: \n" + mob.GetActionQueueText();
        if (mob.Target != null)
            hasLOS.text = "LoS: " + mob.HasLineOfSight(mob.Target.position);
        else
            hasLOS.text = "LoS: null";

    }
}
