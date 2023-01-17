using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListSubmenuHandler : MonoBehaviour
{
    public List<ListSubmenuOptions> MenuOptions = new List<ListSubmenuOptions>();
    public List<GameObject> MenuGameObjects = new List<GameObject>();
    public int SelectedIndex = 0;

    public void Select(bool value)
    {
        MenuGameObjects[SelectedIndex].GetComponentInChildren<Image>().enabled = value;
    }

    public void Invoke() => MenuOptions[SelectedIndex].Invoke();
}
