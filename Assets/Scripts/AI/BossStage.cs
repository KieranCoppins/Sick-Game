using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

[System.Serializable]
public class BossStage
{
    public int healthThreshold;
    [SerializeField] public AbilityBase[] abilities;

    public UnityEvent StartStage;

    public void Update()
    {
    }

}
