using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows for a function to be called every frame whilst we are waiting for the seconds passed
/// </summary>
public class DoTaskWhilstWaitingForSeconds : CustomYieldInstruction
{
    private readonly UnityAction _task;
    private float _timer;

    public override bool keepWaiting
    {
        get
        {
            _task.Invoke();
            _timer -= Time.deltaTime;
            return _timer > 0;
        }
    }

    public DoTaskWhilstWaitingForSeconds(UnityAction task, float seconds)
    {
        _task = task;
        _timer = seconds;
    }
}

/// <summary>
/// Does task every frame until condition is true
/// </summary>
public class DoTaskWhilstWaitingUntil : CustomYieldInstruction
{
    private readonly UnityAction _task;
    private Func<bool> _condition;
    public DoTaskWhilstWaitingUntil(UnityAction task, Func<bool> condition)
    {
        _task = task;
       _condition = condition;
    }

    public override bool keepWaiting
    {
        get
        {
            _task.Invoke();
            return !_condition.Invoke();
        }
    }
}
