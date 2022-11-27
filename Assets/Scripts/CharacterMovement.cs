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

    public List<InventoryItem> inventoryQuickbar = new List<InventoryItem>();
    public List<AbilityBase> abilityQuickbar = new List<AbilityBase>();
    [SerializeField] InventoryRadialMenu inventoryRadialMenu;
    [SerializeField] AbilityRadialMenu abilityRadialMenu;
    [SerializeField] GameObject pauseMenu;

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

        if (context.started && pauseMenu.activeInHierarchy)
            pauseMenu.GetComponentInChildren<InventoryListMenu>().SelectItem(context.ReadValue<Vector2>());
        else if (context.performed && !pauseMenu.activeInHierarchy)
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
        if (context.started)
        {
            if (pauseMenu.activeInHierarchy)
            {
                ListMenu listMenu = pauseMenu.GetComponentInChildren<ListMenu>();
                if (listMenu.IsSubmenuOpen())
                    listMenu.CloseSubmenu();
                else
                    listMenu.OpenSubmenu();
            }
            else if (QueueAttack && Stamina >= 10 && CanMove)
            {
                attackStage++;
                animator.SetInteger("AttackStage", attackStage);
                QueueAttack = false;
                Stamina -= 10;
                rb.velocity = movementVelocity * MovementSpeed;
            }
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
        // If we're using our radial menu, then we want to use the right stick for manouvering the radial menu
        if (!context.canceled)
        {
            if (inventoryRadialMenu.Open)
            {
                inventoryRadialMenu.SelectItem(context.ReadValue<Vector2>());
            }
            if (abilityRadialMenu.Open)
            {

                abilityRadialMenu.SelectItem(context.ReadValue<Vector2>());
            }
        }

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
        if (Mana >= 10 && !animator.GetBool("CastAbility"))
        {
            CanMove = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("CastAbility", true);
        }
    }

    public void CastAbility()
    {
        Mana -= 10;
        selectedAbility.Cast(transform.position, lookAtMouse.LookDirection, Target);
    }

    public void FinishCast()
    {
        animator.SetBool("CastAbility", false);
        CanMove = true;
    }

    public void InventoryRadialMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
            inventoryRadialMenu.Display();

        if (context.canceled)
            inventoryRadialMenu.Close();
    }

    public void AbilityRadialMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
            abilityRadialMenu.Display();

        if (context.canceled)
            abilityRadialMenu.Close();
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

    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
            pauseMenu.GetComponentInChildren<ListMenu>().Display();
        }
    }

    protected override void Die()
    {
        Debug.Log("Player dead");
    }
}
