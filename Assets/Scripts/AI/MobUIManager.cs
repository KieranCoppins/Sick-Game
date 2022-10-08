using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BaseMob))]
public class MobUIManager : MonoBehaviour
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

    // WHEN DID THESE FUNCTIONS EXIST?! - Show UI if our mouse is over the enemy
    // We could also add if a key is down show UI, if we do this move enabling and disabling into a function and call that
    // We disable and enable the coroutine also - theres no point in updating our UI if we cant see it
    private void OnMouseEnter()
    {
        StartCoroutine(UpdateUI());
        canvas.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        StopCoroutine(UpdateUI());
        canvas.gameObject.SetActive(false);
    }
}
