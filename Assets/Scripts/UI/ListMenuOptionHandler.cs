using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ListMenuOptionHandler : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler
{
    Sprite icon;
    string label;
    string description;

    ListMenuOption optionData;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image selectionImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ListMenu menu = GetComponentInParent<ListMenu>();
        ListSubmenuHandler submenuHandler = GetComponentInParent<ListSubmenuHandler>();
        if (menu)
        {
            if (!menu.IsSubmenuOpen())
            {
                menu.SelectItem(optionData);
            }
        }
        else if (submenuHandler)
        {
            foreach (var item in submenuHandler.menuGameObjects)
            {
                ListMenuOptionHandler handler = item.GetComponent<ListMenuOptionHandler>();
                if (handler.label == label)
                {
                    submenuHandler.Select(false);
                    submenuHandler.selectedIndex = submenuHandler.menuGameObjects.IndexOf(item);
                    submenuHandler.Select(true);
                    break;
                }
            }
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ListMenu menu = GetComponentInParent<ListMenu>();
        if (menu.IsSubmenuOpen())
            menu.CloseSubmenu();
        else
            menu.OpenSubmenu();
    }

    public void Select(bool value) => selectionImage.enabled = value;

    public void SetOptionData(ListMenuOption option)
    {
        icon= option.icon;
        label= option.label;
        nameText.text = label;
        description= option.description;
        optionData = option;
    }
}
