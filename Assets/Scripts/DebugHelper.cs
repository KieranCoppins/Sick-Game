using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugHelper : MonoBehaviour
{
    [Header("Press SPACE to Invoke event")]
    [SerializeField] UnityEvent DebugCalls;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DebugCalls.Invoke();
    }
}

/// 
///     BELOW CONTAINS HOW TO IMPLEMENT A DECISION TREE WITH DECISION, DECISION<T> & ACTION NODES.
///     This decision tree system is very script heavy. It uses plenty of generic classes,
///     but it doesn't allow for a non programatic workflow. However, it does mean that
///     it is highly customisable where each action or decision can be unique and made just the way
///     the programmer wants (An issue with Unreal Engine's Decision trees).
///
