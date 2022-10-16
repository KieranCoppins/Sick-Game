using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{

    [SerializeField] float maxDistance;
    [SerializeField] Transform hitTarget;

    Vector3 mousePos;
    public Vector2 LookDirection { get; private set; }

    SpriteRenderer characterRenderer;
    PlayerInput playerInput;
    CharacterMovement characterMovement;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterRenderer = GetComponentInChildren<SpriteRenderer>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        if (characterMovement.Target != null)
        {
            LookDirection = (characterMovement.Target.position - transform.position).normalized;
        }
        else
        {
            if (playerInput.currentControlScheme == "Keyboard&Mouse")
            {
                mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePos.z = 0;

                LookDirection = (mousePos - transform.position).normalized;
            }
        }

        hitTarget.position = transform.position + (Vector3)LookDirection * 1.5f;
        // Flip our weapon and character sprites depending on where we're looking
        if (LookDirection.x < 0)
        {
            characterRenderer.flipX = true;
        }
        else
        {
            characterRenderer.flipX = false;
        }
        Physics2D.Raycast(transform.position, LookDirection, maxDistance);

        Debug.DrawRay(transform.position, LookDirection);
    }

    public void UpdateDirection(InputAction.CallbackContext context)
    {
        if (playerInput.currentControlScheme == "Gamepad" && context.performed && characterMovement.Target == null)
        {
            LookDirection = context.ReadValue<Vector2>().normalized;
        }
    }
}
