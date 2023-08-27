using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public delegate void CharacterInteractable(BaseCharacter character);

// Character movement needs a rigidbody2D component
[RequireComponent(typeof(PlayerInput))]
[DisallowMultipleComponent]
public class CharacterMovement : BaseCharacter
{
    [SerializeField] int Damage;
    [SerializeField] int HeavyDamage;

    [Header("UI Elements")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _staminaBar;
    [SerializeField] private Slider _manaBar;
    [SerializeField] private GameObject _targetGraphic;

    public bool CanMove { get; private set; }
    private Vector2 _movementVelocity;

    private int _attackStage = 0;
    private bool _queueAttack = true;

    public List<AbilityBase> AllAbilities = new List<AbilityBase>();

    [HideInInspector] public List<InventoryItem> InventoryQuickbar = new List<InventoryItem>();
    [HideInInspector] public List<AbilityBase> AbilityQuickbar = new List<AbilityBase>();

    public InventoryItem SelectedItem { get; set; }

    public AbilityBase SelectedAbility { get; set; }

    public CharacterInteractable OnCharacterInteraction;

    /// Base character attribute overrides

    public override int Health
    {
        get => base.Health;
        protected set
        {
            base.Health = value;
            _healthBar.value = (float)Health / (float)MaxHealth;

        }
    }

    public override int Stamina
    {
        get => base.Stamina;
        protected set
        {
            base.Stamina = value;
            _staminaBar.value = (float)Stamina / (float)MaxStamina;
        }
    }

    public override int Mana
    {
        get => base.Mana;
        protected set
        {
            base.Mana = value;
            _manaBar.value = (float)Mana / (float)MaxMana;
        }
    }

    public override int MaxHealth
    {
        get => base.MaxHealth;
        protected set
        {
            base.MaxHealth = value;
            _healthBar.value = (float)Health / (float)MaxHealth;
        }
    }

    public override int MaxStamina
    {
        get => base.MaxStamina;
        protected set
        {
            base.MaxStamina = value;
            _staminaBar.value = (float)Stamina / (float)MaxStamina;
        }
    }

    public override int MaxMana
    {
        get => base.MaxMana;
        protected set
        {
            base.MaxMana = value;
            _manaBar.value = (float)Mana / (float)MaxMana;
        }
    }


    protected override bool Stunned
    {
        get => base.Stunned;
        set
        {
            base.Stunned = value;
            CanMove = !value;
        }
    }

    [Header("Functional References")]
    [SerializeField] private Transform abilityCastPosition;
    private PlayerInput playerInput;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        CanMove = true;

        // Default looking right on start
        LookDirection = new Vector2(1, 0);
        playerInput = GetComponent<PlayerInput>();

    }

    protected override void Update()
    {
        base.Update();
        if (Target != null)
            _targetGraphic.transform.position = Target.transform.position;
        else
            _targetGraphic.SetActive(false);

        Animator.SetBool("Moving", CanMove && RigidBody.velocity.magnitude > 0);
        Animator.SetFloat("Vertical", Mathf.RoundToInt(LookDirection.y));
        Animator.SetFloat("Horizontal", Mathf.RoundToInt(LookDirection.x));
    }

    void FixedUpdate()
    {
        if (CanMove && _attackStage == 0)
        {
            RigidBody.velocity = _movementVelocity * MovementSpeed;
            if (_movementVelocity != Vector2.zero)
                LookDirection = _movementVelocity.normalized;
        }

        Animator.SetFloat("MovementScale", RigidBody.velocity.magnitude / MovementSpeed);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
            _movementVelocity = context.ReadValue<Vector2>();
        else
            _movementVelocity = Vector2.zero;
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.started && CanMove && Stamina >= 10 && _movementVelocity.magnitude != 0)
        {
            StartCoroutine(DoRoll(_movementVelocity.normalized));
            Stamina -= 10;
        }
    }

    IEnumerator DoRoll(Vector2 direction)
    {
        float rollTime = .2f;
        CanMove = false;
        Animator.Play("Roll");
        yield return null;
        while (Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            LookDirection = direction.normalized;
            rollTime -= Time.deltaTime;
            RigidBody.velocity = direction.normalized * RollSpeed;
            yield return null;
        }
        CanMove = true;
        yield return null;
    }


    public override void TakeDamage(BaseCharacter character, int damage)
    {
        Health -= damage;
        StartCoroutine(Stun(0.5f));
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && _queueAttack && Stamina >= 10 && CanMove)
        {
            // Set our look direction to where our mouse is:
            if (playerInput.currentControlScheme == "Keyboard&Mouse")
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePosition.z = 0;
                LookDirection = (mousePosition - transform.position).normalized;

            }

            _attackStage++;
            Animator.Play($"Attack{_attackStage}");
            _queueAttack = false;
            Stamina -= 10;
            RigidBody.velocity = LookDirection * MovementSpeed;
        }
    }
    public void HeavyAttack(InputAction.CallbackContext context)
    {
        if (context.started && _queueAttack && Stamina >= 20 && CanMove)
        {
            // Set our look direction to where our mouse is:
            if (playerInput.currentControlScheme == "Keyboard&Mouse")
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePosition.z = 0;
                LookDirection = (mousePosition - transform.position).normalized;
            }
            Animator.Play($"Heavy Attack");
            Stamina -= 20;
            RigidBody.velocity = LookDirection * MovementSpeed;
            CanMove = false;
        }
    }
    public void ResetAttackStage(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            _attackStage = 0;
            _queueAttack = true;
            CanMove = true;
        }
    }
    public void CanQueueAttack(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            _queueAttack = true;
        }
    }

    public void DealDamage(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll((Vector2)transform.position + LookDirection.normalized, 1.5f);
            foreach (Collider2D target in targets)
            {
                if (target.CompareTag("Mob"))
                {
                    target.GetComponent<BaseMob>().TakeDamage(this, Damage);
                }
            }
        }
    }

    public void DealHeavyDamage(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll((Vector2)transform.position + LookDirection.normalized, 1.5f);
            foreach (Collider2D target in targets)
            {
                if (target.CompareTag("Mob"))
                {
                    target.GetComponent<BaseMob>().TakeDamage(this, HeavyDamage);
                }
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
                    float dotProd = Vector2.Dot((collider.transform.position - transform.position).normalized, LookDirection.normalized) * targetRange * 0.8f;
                    targetWeightPair.Add(new KeyValuePair<Transform, float>(collider.transform, dist + dotProd));
                }
            }

            targetWeightPair.Sort(new KeyValuePairComparer<Transform, float>());
            Target = targetWeightPair[0].Key;
            _targetGraphic.SetActive(true);
        }
        else
        {
            Target = null;
            _targetGraphic.SetActive(false);
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
        if (Mana >= SelectedAbility.ManaCost && CanMove)
        {
            CanMove = false;
            RigidBody.velocity = Vector2.zero;
            Animator.Play("Cast");
        }
    }

    public void CastAbility(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            Mana -= SelectedAbility.ManaCost;
            SelectedAbility.Cast(abilityCastPosition.position, LookDirection, Target, this, 0.0f);
        }
    }

    public void FinishCast(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            CanMove = true;
        }
    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (context.performed && Inventory.Has(SelectedItem))
        {
            Inventory.Use(SelectedItem);
            if (!Inventory.Has(SelectedItem))
            {
                InventoryQuickbar.Remove(SelectedItem);
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCharacterInteraction?.Invoke(this);
        }
    }

    protected override void Die()
    {
        Debug.Log("Player dead");
    }
}
