using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Character movement needs a rigidbody2D component
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LookAtMouse))]
[DisallowMultipleComponent]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rollSpeed = 5f;
    Rigidbody2D rb;

    [Header("Player Stats")]
    [SerializeField] int MaxHealth;
    [SerializeField] int MaxStamina;
    [SerializeField] float StaminaRegenCooldown;
    [SerializeField] int StaminaRegentAmount;
    [SerializeField] int MaxMana;
    [SerializeField] int Damage;

    float StaminaRegenTimer;

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
            // If we deplete our stamina, then we want to initiate our regen calldown
            if (value < _stamina)
                StaminaRegenTimer = StaminaRegenCooldown;

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

    Animator animator;
    int attackStage = 0;
    bool QueueAttack = true;

    [SerializeField] GameObject TargetGraphic;
    public Transform Target { 
        get { return _target; }
        private set
        {
            TargetGraphic.SetActive(value != null);
            _target = value;
        }
    }
    Transform _target;
    LookAtMouse lookAtMouse;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lookAtMouse = GetComponent<LookAtMouse>();

        // We dont want gravity since technically down in unity is at the bottom of the screen
        rb.gravityScale = 0f;

        // Initialise stats
        Health = MaxHealth;
        Stamina = MaxStamina;
        Mana = MaxMana;

        StartCoroutine(RegenStats());
    }

    IEnumerator RegenStats()
    {
        while (true)
        {
            if (StaminaRegenTimer <= 0)
                Stamina += StaminaRegentAmount;

            yield return new WaitForSeconds(0.1f);
        }

    }

    private void Update()
    {
        StaminaRegenTimer -= Time.deltaTime;
        if (Target != null)
            TargetGraphic.transform.position = Target.transform.position;
        else
            TargetGraphic.SetActive(false);

    }

    void FixedUpdate()
    {
        // Create a vector from this and normalise it. Multiply it by the movementSpeed and use this as our velocity for the rigidbody
        if (!rolling && attackStage == 0)
            rb.velocity = movementVelocity * movementSpeed;

        animator.SetBool("Moving", !rolling && rb.velocity.sqrMagnitude > 0);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
            movementVelocity = context.ReadValue<Vector2>();
        else
            movementVelocity = Vector2.zero;

        animator.SetFloat("MovementScale", movementVelocity.magnitude);
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.started && !rolling && Stamina >= 10)
        {
            StartCoroutine(DoRoll(movementVelocity.normalized));
            Stamina -= 10;
        }
    }

    IEnumerator DoRoll(Vector2 direction)
    {
        float rollTime = .2f;
        rolling = true;
        animator.SetBool("Rolling", true);
        while (rollTime > 0f)
        {
            rollTime -= Time.deltaTime;
            rb.velocity = direction * rollSpeed;
            yield return null;
        }
        rolling = false;
        animator.SetBool("Rolling", false);
        yield return null;
    }


    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (QueueAttack && Stamina >= 10)
        {
            rb.velocity = Vector2.zero;
            attackStage++;
            animator.SetInteger("AttackStage", attackStage);
            QueueAttack = false;
            Stamina -= 10;
        }
    }
    public void ResetAttackStage() 
    { 
        attackStage = 0; 
        animator.SetInteger("AttackStage", attackStage);
        QueueAttack = true;
    }
    public void CanQueueAttack() { QueueAttack = true; }

    public void DealDamage()
    {

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        foreach (Collider2D target in targets)
        {
            if (target.CompareTag("Mob"))
            {
                target.GetComponent<BaseMob>().TakeDamage(Damage);
            }
        }
    }

    public void ToggleTargetLock(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (Target == null)
        {
            float targetRange = 14f;
            // Get all our targets in a radius around the player (we dont want to lock onto a target miles away)
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, targetRange / 2f);

            List<KeyValuePair<Transform, float>> targetWeightPair = new List<KeyValuePair<Transform, float>>();

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Mob"))
                {
                    float dist = targetRange - Vector2.Distance(transform.position, collider.transform.position);
                    float dotProd = Vector2.Dot((collider.transform.position - transform.position).normalized, lookAtMouse.LookDirection.normalized) * targetRange * 0.8f;
                    targetWeightPair.Add(new KeyValuePair<Transform, float>(collider.transform, dist + dotProd));
                }
            }

            targetWeightPair.Sort(new KeyValuePairComparer<Transform, float>());
            Target = targetWeightPair[0].Key;
        }
        else
            Target = null;
    }
}
