using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ListSubmenuOptions
{
    public UnityEvent OnSubmenuOptionClicked;
    public string Label;

    public void Invoke()
    {
        OnSubmenuOptionClicked.Invoke();
    }

}
