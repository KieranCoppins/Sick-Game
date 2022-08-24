using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUIHandler : MonoBehaviour
{
    [SerializeField] TMP_Text name;
    [SerializeField] TMP_Text currentAction;
    [SerializeField] TMP_Text actionQueue;

    BaseMob mob;

    void Start()
    {
        mob = GetComponent<BaseMob>();
        name.text = gameObject.name;
    }

    void FixedUpdate()
    {
        currentAction.text = "Current Action(s): " + mob.GetCurrentActionText();
        actionQueue.text = "Action Queue: \n" + mob.GetActionQueueText();
    }
}
