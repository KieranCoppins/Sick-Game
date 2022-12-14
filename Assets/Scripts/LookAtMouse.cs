using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{

    [SerializeField] float maxDistance;
    [SerializeField] Transform hitTarget;

    Vector3 mousePos;

    PlayerInput playerInput;
    CharacterMovement characterMovement;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        // Only update our look direction is we are able to move
        if (characterMovement.CanMove)
        {
            if (characterMovement.Target != null)
            {
                characterMovement.LookDirection = (characterMovement.Target.position - transform.position).normalized;
            }
            else
            {
                if (playerInput.currentControlScheme == "Keyboard&Mouse")
                {
                    mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                    mousePos.z = 0;

                    characterMovement.LookDirection = (mousePos - transform.position).normalized;
                }
            }

            hitTarget.position = transform.position + (Vector3)characterMovement.LookDirection * 1.5f;
        }
    }

    public void UpdateDirection(InputAction.CallbackContext context)
    {
        if (playerInput.currentControlScheme == "Gamepad" && context.performed && characterMovement.Target == null)
        {
            characterMovement.LookDirection = context.ReadValue<Vector2>().normalized;
        }
    }
}
