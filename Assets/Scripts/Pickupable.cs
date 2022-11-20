using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pickupable : MonoBehaviour
{
    [SerializeField] InventoryItem item;
    [SerializeField] Canvas pickupUI;

    private void Start()
    {
        pickupUI.GetComponentInChildren<TextMeshProUGUI>().text= $"Pick up {item.name}";
    }

    public void PickUp(BaseCharacter character)
    {
        character.inventory.Add(item);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Show pick up prompt
            collision.GetComponent<CharacterMovement>().onCharacterInteraction += PickUp;
            pickupUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Remove pick up prompt
            collision.GetComponent<CharacterMovement>().onCharacterInteraction -= PickUp;
            pickupUI.gameObject.SetActive(false);
        }
    }
}
