using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BaseMob))]
public class MobUIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] Slider healthBar;
    [SerializeField] Slider staminaBar; // Dont support stamina just yet

    BaseMob mob;

    private void Awake()
    {
        mob = GetComponent<BaseMob>();
    }

    IEnumerator UpdateUI()
    {
        while (true)
        {
            healthBar.value = (float)mob.Health / (float)mob.MaxHealth;
            yield return null;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(UpdateUI());
        canvas.gameObject.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(UpdateUI());
        canvas.gameObject.SetActive(false);
    }
}
