using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

[System.Serializable]
public class BossStage
{
    public int HealthThreshold;
    [SerializeField] public AbilityBase[] Abilities;

    public UnityEvent StartStage;

    public void Update()
    {
    }

}
