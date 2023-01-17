using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public abstract class ListMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private GameObject _submenu;

    protected List<ListMenuOption> MenuOptions = new List<ListMenuOption>();
    [SerializeField] private GameObject _menuOptionPrefab;

    protected ListMenuOption SelectedOption;

    public abstract void LoadData();

    public void Display()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        LoadData();

        foreach (var item in MenuOptions)
        {
            GameObject go = Instantiate(_menuOptionPrefab, transform);
            ListMenuOptionHandler lmoh = go.GetComponent<ListMenuOptionHandler>();
            lmoh.SetOptionData(item);
            item.GO = go;
        }


        if (MenuOptions.Count > 0)
        {
            SelectItem(MenuOptions[0]);
        }
    }

    public virtual void SelectItem(ListMenuOption option)
    {
        SelectedOption?.GO.GetComponent<ListMenuOptionHandler>().Select(false);
        _itemDescriptionText.text = $"<style=\"H1\">{option.Label}</style>\n{option.Description}";
        _itemIcon.sprite = option.Icon;
        SelectedOption = option;
        option.GO.GetComponent<ListMenuOptionHandler>().Select(true);
    }

    public virtual void SelectItem(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        int currentSelectionIndex = MenuOptions.IndexOf(SelectedOption);
        if (Vector2.Dot(direction.normalized, Vector2.up) > 0)
        {
            if (_submenu != null && IsSubmenuOpen())
            {
                ListSubmenuHandler submenuHandler = _submenu.GetComponentInChildren<ListSubmenuHandler>();
                submenuHandler.Select(false);
                submenuHandler.SelectedIndex--;
                if (submenuHandler.SelectedIndex < 0)
                    submenuHandler.SelectedIndex += submenuHandler.MenuOptions.Count;
                submenuHandler.Select(true);
            }
            else
            {
                currentSelectionIndex--;
                if (currentSelectionIndex < 0)
                    currentSelectionIndex += MenuOptions.Count;

                SelectItem(MenuOptions[currentSelectionIndex]);
            }
        }
        else if (Vector2.Dot(direction.normalized, Vector2.down) > 0)
        {
            if (_submenu != null && IsSubmenuOpen())
            {
                ListSubmenuHandler submenuHandler = _submenu.GetComponentInChildren<ListSubmenuHandler>();
                submenuHandler.Select(false);
                submenuHandler.SelectedIndex++;
                if (submenuHandler.SelectedIndex >= submenuHandler.MenuOptions.Count)
                    submenuHandler.SelectedIndex = 0;
                submenuHandler.Select(true);
            }
            else
            {
                currentSelectionIndex++;
                if (currentSelectionIndex >= MenuOptions.Count)
                    currentSelectionIndex = 0;

                SelectItem(MenuOptions[currentSelectionIndex]);
            }
        }
        
    }

    public void OpenSubmenu()
    {
        if (_submenu != null)
        {
            _submenu.SetActive(true);
            _submenu.GetComponent<RectTransform>().anchoredPosition = SelectedOption.GO.GetComponent<RectTransform>().anchoredPosition;
            ListSubmenuHandler submenuHandler = _submenu.GetComponentInChildren<ListSubmenuHandler>();

            foreach (Transform t in submenuHandler.transform) 
            { 
                Destroy(t.gameObject);
            }
            submenuHandler.MenuGameObjects.Clear();

            foreach (var option in submenuHandler.MenuOptions)
            {
                GameObject go = Instantiate(_menuOptionPrefab, submenuHandler.transform);
                ListMenuOption optionData = new ListMenuOption(option.Label, "", null);
                go.GetComponent<ListMenuOptionHandler>().SetOptionData(optionData);
                submenuHandler.MenuGameObjects.Add(go);
            }
            submenuHandler.Select(true);

        }
    }

    public void CloseSubmenu() 
    {
        if (_submenu != null)
        {
            ListSubmenuHandler submenuHandler = _submenu.GetComponentInChildren<ListSubmenuHandler>();
            submenuHandler.Invoke();
            _submenu.SetActive(false);

            // Refresh our display
            Display();
        }
    }

    public bool IsSubmenuOpen() => _submenu.activeInHierarchy;
}
