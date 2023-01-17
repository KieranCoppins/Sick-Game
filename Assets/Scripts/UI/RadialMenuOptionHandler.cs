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
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Image _icon;

    private RadialMenuOption _optionData;

    private Vector3 _desiredPos;

    private RectTransform _rect;

    public void SetOptionData(RadialMenuOption optionData)
    {
        _optionData = optionData;
        _label.text = optionData.Label;
        _icon.sprite = optionData.Icon;
    }
    public void SetPosition(Vector3 pos) => _desiredPos = pos;
    public Vector3 GetPosition() => _desiredPos;
    public RadialMenuOption GetOptionData() => _optionData;

    private void OnEnable()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rect.anchoredPosition = Vector3.Lerp(_rect.anchoredPosition, _desiredPos, 0.03f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<RadialMenu>().SelectItem(_optionData);
    }
}
