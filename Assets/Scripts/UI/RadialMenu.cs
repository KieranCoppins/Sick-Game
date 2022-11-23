using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public abstract class RadialMenu : MonoBehaviour
{
    [SerializeField] float radius;
    public List<RadialMenuOption> menuOptions = new List<RadialMenuOption>();
    List<GameObject> openOptions = new List<GameObject>();
    [SerializeField] GameObject menuOptionPrefab;

    [SerializeField] TextMeshProUGUI descriptionLabel;

    protected RadialMenuOption selectedOption;

    public bool Open { get; private set; }

    public abstract void LoadData();

    public void Display()
    {
        menuOptions.Clear();
        LoadData();
        if (menuOptions.Count == 0)
            return;

        Open = true;
        gameObject.SetActive(true);
        float radianSeperation = (Mathf.PI * 2) / menuOptions.Count;

        for(int i = 0; i < menuOptions.Count; i++)
        {
            Vector3 offset = new Vector3(Mathf.Sin(radianSeperation * i) * radius, Mathf.Cos(radianSeperation * i) * radius);
            GameObject go = Instantiate(menuOptionPrefab, transform);
            openOptions.Add(go);
            RadialMenuOptionHandler oh = go.GetComponent<RadialMenuOptionHandler>();
            oh.SetOptionData(menuOptions[i]);
            oh.SetPosition(offset);
        }

        SelectItem(menuOptions[0]);
    }

    public virtual void Close()
    {
        if (Open)
        {
            Open = false;
            StartCoroutine(CloseAnimated());
        }
    }

    IEnumerator CloseAnimated()
    {
        for (int i = 0; i < openOptions.Count; i++)
        {
            RadialMenuOptionHandler oh = openOptions[i].GetComponent<RadialMenuOptionHandler>();
            oh.SetPosition(Vector3.zero);
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < openOptions.Count; i++)
        {
            Destroy(openOptions[i]);
        }
        openOptions.Clear();
        gameObject.SetActive(false);

        yield return null;
    }

    /// <summary>
    /// Selects the passed object
    /// </summary>
    /// <param name="option"></param>
    public virtual void SelectItem(RadialMenuOption option)
    {
        descriptionLabel.text = $"<style=\"H1\">{option.label}</style>\n{option.description}";
        selectedOption = option;
    }

    /// <summary>
    /// Selects an object based on a direction vector that is pointing towards the option from the menu origin
    /// </summary>
    /// <param name="direction"></param>
    public virtual void SelectItem(Vector2 direction)
    {
        float bestDot = -1;
        GameObject option = null;
        foreach(var handler in openOptions)
        {
            Vector2 pos = handler.GetComponent<RadialMenuOptionHandler>().GetPosition();
            float dot = Vector2.Dot(pos.normalized, direction.normalized);
            if (dot > bestDot) 
            {
                bestDot = dot;
                option = handler;
            }
        }
        SelectItem(option.GetComponent<RadialMenuOptionHandler>().GetOptionData());
    }


}
