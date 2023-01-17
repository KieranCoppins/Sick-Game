using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ListMenuOptionHandler : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler
{
    private string _label;

    private ListMenuOption _optionData;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _selectionImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ListMenu menu = GetComponentInParent<ListMenu>();
        ListSubmenuHandler submenuHandler = GetComponentInParent<ListSubmenuHandler>();
        if (menu)
        {
            if (!menu.IsSubmenuOpen())
            {
                menu.SelectItem(_optionData);
            }
        }
        else if (submenuHandler)
        {
            foreach (var item in submenuHandler.MenuGameObjects)
            {
                ListMenuOptionHandler handler = item.GetComponent<ListMenuOptionHandler>();
                if (handler._label == _label)
                {
                    submenuHandler.Select(false);
                    submenuHandler.SelectedIndex = submenuHandler.MenuGameObjects.IndexOf(item);
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

    public void Select(bool value) => _selectionImage.enabled = value;

    public void SetOptionData(ListMenuOption option)
    {
        _label= option.Label;
        _nameText.text = _label;
        _optionData = option;
    }
}
