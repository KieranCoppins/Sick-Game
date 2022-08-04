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
