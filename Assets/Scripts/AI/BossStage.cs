using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BossStage
{
    public int healthThreshold;
    public AIAbilityBase[] abilities;

    public UnityEvent StartStage;

}
