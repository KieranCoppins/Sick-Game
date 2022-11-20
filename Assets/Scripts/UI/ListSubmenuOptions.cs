using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ListSubmenuOptions
{
    public UnityEvent onSubmenuOptionClicked;
    public string label;

    public void Invoke()
    {
        onSubmenuOptionClicked.Invoke();
    }

}
