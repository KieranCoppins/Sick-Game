using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public abstract class ListMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    [SerializeField] Image itemIcon;
    [SerializeField] GameObject submenu;

    protected List<ListMenuOption> menuOptions = new List<ListMenuOption>();
    [SerializeField] GameObject menuOptionPrefab;

    protected ListMenuOption SelectedOption;

    public abstract void LoadData();

    public void Display()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        LoadData();

        foreach (var item in menuOptions)
        {
            GameObject go = Instantiate(menuOptionPrefab, transform);
            ListMenuOptionHandler lmoh = go.GetComponent<ListMenuOptionHandler>();
            lmoh.SetOptionData(item);
            item.go = go;
        }


        if (menuOptions.Count > 0)
        {
            SelectItem(menuOptions[0]);
        }
    }

    public virtual void SelectItem(ListMenuOption option)
    {
        SelectedOption?.go.GetComponent<ListMenuOptionHandler>().Select(false);
        itemDescriptionText.text = $"<style=\"H1\">{option.label}</style>\n{option.description}";
        itemIcon.sprite = option.icon;
        SelectedOption = option;
        option.go.GetComponent<ListMenuOptionHandler>().Select(true);
    }

    public virtual void SelectItem(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        int currentSelectionIndex = menuOptions.IndexOf(SelectedOption);
        if (Vector2.Dot(direction.normalized, Vector2.up) > 0)
        {
            if (submenu != null && IsSubmenuOpen())
            {
                ListSubmenuHandler submenuHandler = submenu.GetComponentInChildren<ListSubmenuHandler>();
                submenuHandler.Select(false);
                submenuHandler.selectedIndex--;
                if (submenuHandler.selectedIndex < 0)
                    submenuHandler.selectedIndex += submenuHandler.menuOptions.Count;
                submenuHandler.Select(true);
            }
            else
            {
                currentSelectionIndex--;
                if (currentSelectionIndex < 0)
                    currentSelectionIndex += menuOptions.Count;

                SelectItem(menuOptions[currentSelectionIndex]);
            }
        }
        else if (Vector2.Dot(direction.normalized, Vector2.down) > 0)
        {
            if (submenu != null && IsSubmenuOpen())
            {
                ListSubmenuHandler submenuHandler = submenu.GetComponentInChildren<ListSubmenuHandler>();
                submenuHandler.Select(false);
                submenuHandler.selectedIndex++;
                if (submenuHandler.selectedIndex >= submenuHandler.menuOptions.Count)
                    submenuHandler.selectedIndex = 0;
                submenuHandler.Select(true);
            }
            else
            {
                currentSelectionIndex++;
                if (currentSelectionIndex >= menuOptions.Count)
                    currentSelectionIndex = 0;

                SelectItem(menuOptions[currentSelectionIndex]);
            }
        }
        
    }

    public void OpenSubmenu()
    {
        if (submenu != null)
        {
            submenu.SetActive(true);
            submenu.GetComponent<RectTransform>().anchoredPosition = SelectedOption.go.GetComponent<RectTransform>().anchoredPosition;
            ListSubmenuHandler submenuHandler = submenu.GetComponentInChildren<ListSubmenuHandler>();

            foreach (Transform t in submenuHandler.transform) 
            { 
                Destroy(t.gameObject);
            }
            submenuHandler.menuGameObjects.Clear();

            foreach (var option in submenuHandler.menuOptions)
            {
                GameObject go = Instantiate(menuOptionPrefab, submenuHandler.transform);
                ListMenuOption optionData = new ListMenuOption(option.label, "", null);
                go.GetComponent<ListMenuOptionHandler>().SetOptionData(optionData);
                submenuHandler.menuGameObjects.Add(go);
            }
            submenuHandler.Select(true);

        }
    }

    public void CloseSubmenu() 
    {
        if (submenu != null)
        {
            ListSubmenuHandler submenuHandler = submenu.GetComponentInChildren<ListSubmenuHandler>();
            submenuHandler.Invoke();
            submenu.SetActive(false);

            // Refresh our display
            Display();
        }
    }

    public bool IsSubmenuOpen() => submenu.activeInHierarchy;
}
