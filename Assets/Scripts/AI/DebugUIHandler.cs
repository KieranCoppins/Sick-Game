using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUIHandler : MonoBehaviour
{
    [SerializeField] Canvas debugCanvas;
    [SerializeField] TMP_Text name;
    [SerializeField] TMP_Text currentAction;
    [SerializeField] TMP_Text actionQueue;
    [SerializeField] TMP_Text hasLOS;

    BaseMob mob;

    void Start()
    {
        mob = GetComponent<BaseMob>();
        name.text = gameObject.name;
        debugCanvas.enabled = mob.DebugMode;
    }

    void FixedUpdate()
    {
        if (!mob.DebugMode)
            return;

        currentAction.text = "Current Action(s): " + mob.GetCurrentActionText();
        actionQueue.text = "Action Queue: \n" + mob.GetActionQueueText();
        hasLOS.text = "LoS: " + mob.HasLineOfSight(GameObject.FindGameObjectWithTag("Player").transform.position);
    }
}
