using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pickupable : MonoBehaviour
{
    [SerializeField] private InventoryItem _item;
    [SerializeField] private Canvas _pickupUI;

    private void Start()
    {
        _pickupUI.GetComponentInChildren<TextMeshProUGUI>().text= $"Pick up {_item.name}";
    }

    public void PickUp(BaseCharacter character)
    {
        character.Inventory.Add(_item);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Show pick up prompt
            collision.GetComponent<CharacterMovement>().OnCharacterInteraction += PickUp;
            _pickupUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Remove pick up prompt
            collision.GetComponent<CharacterMovement>().OnCharacterInteraction -= PickUp;
            _pickupUI.gameObject.SetActive(false);
        }
    }
}
