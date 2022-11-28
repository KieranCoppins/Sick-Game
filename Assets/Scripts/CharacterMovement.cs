using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public delegate void CharacterInteractable(BaseCharacter character);

// Character movement needs a rigidbody2D component
[RequireComponent(typeof(LookAtMouse))]
[DisallowMultipleComponent]
public class CharacterMovement : BaseCharacter
{
    [SerializeField] int Damage;

    [Header("UI Elements")]
    [SerializeField] Slider HealthBar;
    [SerializeField] Slider StaminaBar;
    [SerializeField] Slider ManaBar;
    [SerializeField] GameObject TargetGraphic;

    bool CanMove = true; 
    Vector2 movementVelocity;

    int attackStage = 0;
    bool QueueAttack = true;

    LookAtMouse lookAtMouse;

    public List<AbilityBase> AllAbilities = new List<AbilityBase>();

    public List<InventoryItem> inventoryQuickbar = new List<InventoryItem>();
    public List<AbilityBase> abilityQuickbar = new List<AbilityBase>();

    public InventoryItem selectedItem;
    public AbilityBase selectedAbility;

    public CharacterInteractable onCharacterInteraction;

    /// Base character attribute overrides

    public override int Health { 
        get => base.Health;
        protected set
        {
            base.Health = value;
            HealthBar.value = (float)Health / (float)MaxHealth;

        }
    }

    public override int Stamina { 
        get => base.Stamina;
        protected set 
        { 
            base.Stamina = value; 
            StaminaBar.value = (float)Stamina / (float)MaxStamina;
        }
    }

    public override int Mana { 
        get => base.Mana;
        protected set
        {
            base.Mana = value;
            ManaBar.value = (float)Mana / (float)MaxMana;
        }
    }

    public override int MaxHealth { 
        get => base.MaxHealth;
        protected set
        {
            base.MaxHealth = value;
            HealthBar.value = (float)Health / (float)MaxHealth;
        }
    }

    public override int MaxStamina 
    { 
        get => base.MaxStamina;
        protected set
        {
            base.MaxStamina = value;
            StaminaBar.value = (float)Stamina / (float)MaxStamina;
        }
    }

    public override int MaxMana { 
        get => base.MaxMana;
        protected set
        {
            base.MaxMana = value;
            ManaBar.value = (float)Mana / (float)MaxMana;
        }
    }


    protected override bool Stunned { 
        get => base.Stunned;
        set
        {
            base.Stunned = value;
            CanMove = !value;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lookAtMouse = GetComponent<LookAtMouse>();
    }

    protected override void Update()
    {
        base.Update();
        if (Target != null)
            TargetGraphic.transform.position = Target.transform.position;
        else
            TargetGraphic.SetActive(false);

        animator.SetBool("Moving", CanMove && rb.velocity.magnitude > 0);
    }

    void FixedUpdate()
    {
        if (CanMove && attackStage == 0)
            rb.velocity = movementVelocity * MovementSpeed;

        animator.SetFloat("MovementScale", rb.velocity.magnitude / MovementSpeed);
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
        if (context.started && CanMove && Stamina >= 10)
        {
            StartCoroutine(DoRoll(movementVelocity.normalized));
            Stamina -= 10;
        }
    }

    IEnumerator DoRoll(Vector2 direction)
    {
        float rollTime = .2f;
        CanMove = false;
        animator.SetBool("Rolling", true);
        while (rollTime > 0f)
        {
            rollTime -= Time.deltaTime;
            rb.velocity = direction * rollSpeed;
            yield return null;
        }
        CanMove = true;
        animator.SetBool("Rolling", false);
        yield return null;
    }


    public void TakeDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(Stun(0.5f));
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && QueueAttack && Stamina >= 10 && CanMove)
        {
            attackStage++;
            animator.SetInteger("AttackStage", attackStage);
            QueueAttack = false;
            Stamina -= 10;
            rb.velocity = movementVelocity * MovementSpeed;   
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
            TargetGraphic.SetActive(true);
        }
        else
        {
            Target = null;
            TargetGraphic.SetActive(false);
        }
    }


    public void SwitchTarget(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (Target != null)
        {
            Vector2 nextTargetDir = context.ReadValue<Vector2>();
            float targetRange = 14f;
            // Get all our targets in a radius around the player (we dont want to lock onto a target miles away)
            Collider2D[] colliders = Physics2D.OverlapCircleAll(Target.transform.position, targetRange / 2f);

            List<KeyValuePair<Transform, float>> targetWeightPair = new List<KeyValuePair<Transform, float>>();

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Mob"))
                {
                    float dist = targetRange - Vector2.Distance(Target.transform.position, collider.transform.position);
                    float dotProd = Vector2.Dot((collider.transform.position - Target.transform.position).normalized, nextTargetDir.normalized) * targetRange * 0.8f;
                    targetWeightPair.Add(new KeyValuePair<Transform, float>(collider.transform, dist + dotProd));
                }
            }

            targetWeightPair.Sort(new KeyValuePairComparer<Transform, float>());
            Target = targetWeightPair[0].Key;
        }
    }

    public void DoCast(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;
        if (Mana >= selectedAbility.ManaCost && !animator.GetBool("CastAbility"))
        {
            CanMove = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("CastAbility", true);
        }
    }

    public void CastAbility()
    {
        Mana -= selectedAbility.ManaCost;
        selectedAbility.Cast(transform.position, lookAtMouse.LookDirection, Target);
    }

    public void FinishCast()
    {
        animator.SetBool("CastAbility", false);
        CanMove = true;
    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (context.performed && inventory.Has(selectedItem))
        {
            inventory.Use(selectedItem);
            if (!inventory.Has(selectedItem))
            {
                inventoryQuickbar.Remove(selectedItem);
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onCharacterInteraction?.Invoke(this);
        }
    }

    protected override void Die()
    {
        Debug.Log("Player dead");
    }
}
