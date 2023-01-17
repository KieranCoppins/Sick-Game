using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public abstract class RadialMenu : MonoBehaviour
{
    [SerializeField] private float _radius;
    public List<RadialMenuOption> MenuOptions = new List<RadialMenuOption>();
    private List<GameObject> _openOptions = new List<GameObject>();
    [SerializeField] private GameObject _menuOptionPrefab;

    [SerializeField] private TextMeshProUGUI _descriptionLabel;

    protected RadialMenuOption SelectedOption;

    public bool Open { get; private set; }

    public abstract void LoadData();

    public void Display()
    {
        MenuOptions.Clear();
        LoadData();
        if (MenuOptions.Count == 0)
            return;

        Open = true;
        gameObject.SetActive(true);
        float radianSeperation = (Mathf.PI * 2) / MenuOptions.Count;

        for(int i = 0; i < MenuOptions.Count; i++)
        {
            Vector3 offset = new Vector3(Mathf.Sin(radianSeperation * i) * _radius, Mathf.Cos(radianSeperation * i) * _radius);
            GameObject go = Instantiate(_menuOptionPrefab, transform);
            _openOptions.Add(go);
            RadialMenuOptionHandler oh = go.GetComponent<RadialMenuOptionHandler>();
            oh.SetOptionData(MenuOptions[i]);
            oh.SetPosition(offset);
        }

        SelectItem(MenuOptions[0]);
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
        for (int i = 0; i < _openOptions.Count; i++)
        {
            RadialMenuOptionHandler oh = _openOptions[i].GetComponent<RadialMenuOptionHandler>();
            oh.SetPosition(Vector3.zero);
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < _openOptions.Count; i++)
        {
            Destroy(_openOptions[i]);
        }
        _openOptions.Clear();
        gameObject.SetActive(false);

        yield return null;
    }

    /// <summary>
    /// Selects the passed object
    /// </summary>
    /// <param name="option"></param>
    public virtual void SelectItem(RadialMenuOption option)
    {
        _descriptionLabel.text = $"<style=\"H1\">{option.Label}</style>\n{option.Description}";
        SelectedOption = option;
    }

    /// <summary>
    /// Selects an object based on a direction vector that is pointing towards the option from the menu origin
    /// </summary>
    /// <param name="direction"></param>
    public virtual void SelectItem(Vector2 direction)
    {
        float bestDot = -1;
        GameObject option = null;
        foreach(var handler in _openOptions)
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
