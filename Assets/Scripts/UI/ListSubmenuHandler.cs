using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListSubmenuHandler : MonoBehaviour
{
    public List<ListSubmenuOptions> menuOptions = new List<ListSubmenuOptions>();
    public List<GameObject> menuGameObjects = new List<GameObject>();
    public int selectedIndex = 0;

    public void Select(bool value)
    {
        menuGameObjects[selectedIndex].GetComponentInChildren<Image>().enabled = value;
    }

    public void Invoke() => menuOptions[selectedIndex].Invoke();
}
