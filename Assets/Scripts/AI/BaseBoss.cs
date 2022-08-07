using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoss : BaseMob
{

    // We need a system of determining stages of the boss battle -
    // different stages will be based on health, stages can change what abilities they use and overall change their combat style

    [SerializeField] BossStage[] stages;

    public int CurrentStage
    {
        get { return _currentStage; }
        set
        {
            _currentStage = value;
            if (_currentStage < 0)
            {
                _currentStage = 0;
            }
            if (_currentStage > stages.Length - 1)
            {
                _currentStage = stages.Length - 1;
            }
        }
    }

    private int _currentStage = 0;

    protected override void Update()
    {
        base.Update();

        // Move to our next stage if our health is below the threshold
        if (Health < stages[CurrentStage].healthThreshold)
        {
            CurrentStage++;
        }
    }

    public override void Attack(GameObject target)
    {

    }
}
