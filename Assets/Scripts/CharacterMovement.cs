using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Character movement needs a rigidbody2D component
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rollSpeed = 5f;
    Rigidbody2D rb;

    [Header("Player Stats")]
    [SerializeField] int MaxHealth;
    [SerializeField] int MaxStamina;
    [SerializeField] int MaxMana;    

    public int Health { 
        get { return _health; } 
        private set 
        { 
            _health = Mathf.Clamp(value, 0, MaxHealth);
            // Update health UI
            HealthBar.value = (float)_health / (float)MaxHealth;

            if (_health <= 0)
            {

                // Enter death code
            }

        } 
    }
    public int Stamina { 
        get { return _stamina; } 
        private set 
        { 
            _stamina = Mathf.Clamp(value, 0, MaxStamina); 

            // Update stamina UI
            StaminaBar.value = (float)_stamina / (float)MaxStamina;
        } 
    }
    public int Mana { 
        get { return _mana; } 
        private set 
        { 
            _mana = Mathf.Clamp(value, 0, MaxMana); 

            // Update mana UI
            ManaBar.value = (float)_mana / (float)MaxMana;
        } 
    }

    int _health;
    int _stamina;
    int _mana;

    [Header("UI Elements")]
    [SerializeField] Slider HealthBar;
    [SerializeField] Slider StaminaBar;
    [SerializeField] Slider ManaBar;

    bool rolling = false; 
    Vector2 movementVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // We dont want gravity since technically down in unity is at the bottom of the screen
        rb.gravityScale = 0f;

        // Initialise stats
        Health = MaxHealth;
        Stamina = MaxStamina;
        Mana = MaxMana;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Create a vector from this and normalise it. Multiply it by the movementSpeed and use this as our velocity for the rigidbody
        if (!rolling)
            rb.velocity = movementVelocity * movementSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
            movementVelocity = context.ReadValue<Vector2>();
        else
            movementVelocity = Vector2.zero;
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.started && !rolling)
        {
            StartCoroutine(DoRoll(movementVelocity.normalized));
            Stamina -= 10;
        }
    }

    IEnumerator DoRoll(Vector2 direction)
    {
        float rollTime = .2f;
        rolling = true;
        while (rollTime > 0f)
        {
            rollTime -= Time.deltaTime;
            rb.velocity = direction * rollSpeed;
            yield return null;
        }
        rolling = false;
        yield return null;
    }


    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}
