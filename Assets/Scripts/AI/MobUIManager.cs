using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BaseMob))]
public class MobUIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _staminaBar;

    private BaseMob Mob;

    private void Awake()
    {
        Mob = GetComponent<BaseMob>();
    }

    IEnumerator UpdateUI()
    {
        while (true)
        {
            _healthBar.value = (float)Mob.Health / (float)Mob.MaxHealth;
            yield return null;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        EnableUI();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        DisableUI();
    }

    public void EnableUI()
    {
        StartCoroutine(UpdateUI());
        _canvas.gameObject.SetActive(true);
    }

    public void DisableUI()
    {
        StopCoroutine(UpdateUI());
        _canvas.gameObject.SetActive(false);
    }
}
