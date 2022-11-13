using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] int _movementSpeed;
    [SerializeField] protected float rollSpeed = 5f;
    public virtual int MaxHealth { get { return _maxHealth; } protected set { _maxHealth = value; } }
    [Header("Stats")]
    [SerializeField] int _maxHealth;
    public virtual int MaxStamina { get { return _maxStamina; } protected set { _maxStamina = value; } }
    [SerializeField] int _maxStamina;
    [SerializeField] protected float StaminaRegenCooldown;
    [SerializeField] protected int StaminaRegentAmount;
    public virtual int MaxMana { get { return _maxMana; } protected set { _maxMana = value; } }
    [SerializeField] int _maxMana;

    float StaminaRegenTimer;

    public virtual int Health
    {
        get { return _health; }
        protected set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);

            if (_health <= 0)
            {
                // Enter death code

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
}
