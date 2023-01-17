using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField] private Transform _hitTarget;

    private Vector3 _mousePos;

    private PlayerInput _playerInput;
    private CharacterMovement _characterMovement;


    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        // Only update our look direction is we are able to move
        if (_characterMovement.CanMove)
        {
            if (_characterMovement.Target != null)
            {
                _characterMovement.LookDirection = (_characterMovement.Target.position - transform.position).normalized;
            }
            else
            {
                if (_playerInput.currentControlScheme == "Keyboard&Mouse")
                {
                    _mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                    _mousePos.z = 0;

                    _characterMovement.LookDirection = (_mousePos - transform.position).normalized;
                }
            }

            _hitTarget.position = transform.position + (Vector3)_characterMovement.LookDirection * 1.5f;
        }
    }

    public void UpdateDirection(InputAction.CallbackContext context)
    {
        if (_playerInput.currentControlScheme == "Gamepad" && context.performed && _characterMovement.Target == null)
        {
            _characterMovement.LookDirection = context.ReadValue<Vector2>().normalized;
        }
    }
}
