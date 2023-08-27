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
    [SerializeField] private int _movementSpeed;
    [Tooltip("How quickly does this character roll/dodge?")]
    [SerializeField] protected float RollSpeed = 5f;

    public Factions Faction { get { return _faction; } protected set { _faction = value; } }

    [SerializeField] private Factions _faction;

    public virtual int MaxHealth { get { return _maxHealth; } protected set { _maxHealth = value; Health = Health; } }

    [Header("Stats")]
    [Tooltip("The maximum health this character can have")]
    [SerializeField] private int _maxHealth;
    public virtual int MaxStamina { get { return _maxStamina; } protected set { _maxStamina = value; Stamina = Stamina; } }

    [Tooltip("The maximum stamina this character can have")]
    [SerializeField] private int _maxStamina;

    [Tooltip("How long does the character have to go without using any stamina to start naturally regnerating (in seconds)")]
    [SerializeField] protected float StaminaRegenCooldown;

    [Tooltip("The amount this character naturally regenerates their stamina every 100ms")]
    [SerializeField] protected int StaminaRegentAmount;

    public virtual int MaxMana { get { return _maxMana; } protected set { _maxMana = value; Mana = Mana; } }

    [Tooltip("The maximum mana this character can have")]
    [SerializeField] private int _maxMana;

    public int Aggression { get { return _aggression; } protected set { _aggression = value; } }

    [Tooltip("The aggression of this character, this affects how mobs will agro the character in a damage-based aggression system")]
    [SerializeField] private int _aggression = 1;

    private float _staminaRegenTimer;

    public virtual int Health
    {
        get { return _health; }
        protected set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);

            if (_health <= 0)
            {
                if (Animator != null)
                    Animator.Play("Die");
                else
                    Die();
            }
        }
    }

    private int _health;

    public virtual int Stamina
    {
        get { return _stamina; }
        protected set
        {
            // If we deplete our stamina, then we want to initiate our regen calldown
            if (value < _stamina)
                _staminaRegenTimer = StaminaRegenCooldown;

            _stamina = Mathf.Clamp(value, 0, MaxStamina);
        }
    }

    private int _stamina;

    public virtual int Mana
    {
        get { return _mana; }
        protected set
        {
            _mana = Mathf.Clamp(value, 0, MaxMana);
        }
    }

    private int _mana;

    public virtual Transform Target
    {
        get { return _target; }
        protected set
        {
            _target = value;
        }
    }

    private Transform _target;


    public Vector2 LookDirection { get; set; }

    public Rigidbody2D RigidBody { get; protected set; }

    public Animator Animator { get; protected set; }

    public InventorySystem Inventory { get; protected set; }

    public SpriteRenderer SpriteRenderer { get; protected set; }

    protected virtual bool Stunned { get; set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Inventory = GetComponent<InventorySystem>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // We dont want gravity since technically down in unity is at the bottom of the screen
        RigidBody.gravityScale = 0f;

        // Initialise stats
        Health = MaxHealth;
        Stamina = MaxStamina;
        Mana = MaxMana;

        StartCoroutine(RegenStats());
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _staminaRegenTimer -= Time.deltaTime;
    }

    private IEnumerator RegenStats()
    {
        while (true)
        {
            if (_staminaRegenTimer <= 0)
                Stamina += StaminaRegentAmount;

            yield return new WaitForSeconds(0.1f);
        }

    }

    protected IEnumerator Stun(float time)
    {
        Stunned = true;
        RigidBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(time);
        Stunned = false;
    }

    protected abstract void Die();

    public abstract void TakeDamage(BaseCharacter character, int damage);
}
