using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{

    [SerializeField] float maxDistance;
    [SerializeField] Transform hitTarget;

    Vector3 mousePos;
    Vector2 direction;

    SpriteRenderer characterRenderer;

    PlayerInput playerInput;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;

            direction = (mousePos - transform.position).normalized;
        }
        hitTarget.position = transform.position + (Vector3)direction * 1.5f;
        // Flip our weapon and character sprites depending on where we're looking
        if (direction.x < 0)
        {
            characterRenderer.flipX = true;
        } 
        else
        {
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
