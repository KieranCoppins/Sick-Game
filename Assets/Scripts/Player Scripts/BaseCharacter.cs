using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Factions
{
    Ally,
    Enemy
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InventorySystem))]
[DisallowMultipleComponent]
public abstract class BaseCharacter : MonoBehaviour
{
    public int MovementSpeed
    {
        get { return _movementSpeed; }
        protected set { _movementSpeed = value; }
    }

    [Tooltip("How quickly does this character move?")]
    [SerializeField] int _movementSpeed;
    [Tooltip("How quickly does this character roll/dodge?")]
    [SerializeField] protected float rollSpeed = 5f;

    public Factions Faction { get { return _Faction; } protected set { _Faction = value; } }
    [SerializeField] Factions _Faction;

    public virtual int MaxHealth { get { return _maxHealth; } protected set { _maxHealth = value; Health = Health; } }
    [Header("Stats")]
    [Tooltip("The maximum health this character can have")]
    [SerializeField] int _maxHealth;
    public virtual int MaxStamina { get { return _maxStamina; } protected set { _maxStamina = value; Stamina = Stamina; } }
    [Tooltip("The maximum stamina this character can have")]
    [SerializeField] int _maxStamina;
    [Tooltip("How long does the character have to go without using any stamina to start naturally regnerating (in seconds)")]
    [SerializeField] protected float StaminaRegenCooldown;
    [Tooltip("The amount this character naturally regenerates their stamina every 100ms")]
    [SerializeField] protected int StaminaRegentAmount;
    public virtual int MaxMana { get { return _maxMana; } protected set { _maxMana = value; Mana = Mana; } }
    [Tooltip("The maximum mana this character can have")]
    [SerializeField] int _maxMana;
    public int Aggression { get { return _Aggression; } protected set { _Aggression = value; } }
    [Tooltip("The aggression of this character, this affects how mobs will agro the character in a damage-based aggression system")]
    [SerializeField] int _Aggression = 1;

    float StaminaRegenTimer;

    public virtual int Health
    {
        get { return _health; }
        protected set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);

            if (_health <= 0)
            {
                Die();
            }
        }
    }
    int _health;
    public virtual int Stamina
    {
        get { return _stamina; }
        protected set
        {
            // If we deplete our stamina, then we want to initiate our regen calldown
            if (value < _stamina)
                StaminaRegenTimer = StaminaRegenCooldown;

            _stamina = Mathf.Clamp(value, 0, MaxStamina);
        }
    }
    int _stamina;
    public virtual int Mana
    {
        get { return _mana; }
        protected set
        {
            _mana = Mathf.Clamp(value, 0, MaxMana);
        }
    }
    int _mana;

    public virtual Transform Target
    {
        get { return _target; }
        protected set
        {
            _target = value;
        }
    }
    Transform _target;

    public Rigidbody2D rb { get; protected set; }
    protected Animator animator;
    public InventorySystem inventory { get; protected set; }

    protected virtual bool Stunned { get; set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<InventorySystem>();

        // We dont want gravity since technically down in unity is at the bottom of the screen
        rb.gravityScale = 0f;

        // Initialise stats
        Health = MaxHealth;
        Stamina = MaxStamina;
        Mana = MaxMana;

        StartCoroutine(RegenStats());
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        StaminaRegenTimer -= Time.deltaTime;
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

    protected IEnumerator Stun(float time)
    {
        Stunned = true;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(time);
        Stunned = false;
    }

    protected abstract void Die();
}
