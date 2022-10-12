using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{

    [SerializeField] float maxDistance;
    [SerializeField] GameObject weaponGO;

    Vector3 mousePos;
    Vector2 direction;

    SpriteRenderer weaponRenderer;
    SpriteRenderer characterRenderer;

    PlayerInput playerInput;


    private void Start()
    {
        weaponRenderer = weaponGO.GetComponentInChildren<SpriteRenderer>();
        characterRenderer = GetComponent<SpriteRenderer>();

        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;

            direction = (mousePos - transform.position).normalized;
        }

        weaponGO.transform.position = (Vector2)transform.position + direction * 0.5f;

        // Flip our weapon and character sprites depending on where we're looking
        if (direction.x < 0)
        {
            weaponRenderer.flipX = true;
            characterRenderer.flipX = true;
        } 
        else
        {
            weaponGO.GetComponentInChildren<SpriteRenderer>().flipX = false;
            characterRenderer.flipX = false;
        }
        Physics2D.Raycast(transform.position, direction, maxDistance);
        
        Debug.DrawRay(transform.position, direction);
    }

    public void UpdateDirection(InputAction.CallbackContext context)
    {
        if (playerInput.currentControlScheme == "Gamepad" && context.performed)
        {
            direction = context.ReadValue<Vector2>().normalized;
        }
    }
}
