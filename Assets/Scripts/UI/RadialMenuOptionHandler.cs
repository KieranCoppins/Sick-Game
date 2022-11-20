using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public delegate void OptionClickedDelegate();

public class RadialMenuOptionHandler : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] Image icon;

    RadialMenuOption optionData;

    Vector3 desiredPos;

    RectTransform rect;

    public void SetOptionData(RadialMenuOption optionData)
    {
        this.optionData = optionData;
        label.text = optionData.label;
        icon.sprite = optionData.icon;
    }
    public void SetPosition(Vector3 pos) => desiredPos = pos;
    public Vector3 GetPosition() => desiredPos;
    public RadialMenuOption GetOptionData() => optionData;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, desiredPos, 0.03f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<RadialMenu>().SelectItem(optionData);
    }
}
