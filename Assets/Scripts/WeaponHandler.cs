using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float attackRange;
    [SerializeField] float attackSpeed;


    [SerializeField] UnityEvent onAttack;

    Animator animator;

    bool attacking;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!attacking)
            StartCoroutine(DoAttack());
    }

    IEnumerator DoAttack()
    {
        Debug.Log("Attacking");
        attacking = true;
        animator.SetBool("Attacking", true);
        yield return null;
        animator.SetBool("Attacking", false);
        onAttack?.Invoke();
        yield return new WaitForSeconds(attackSpeed);
        attacking = false;
        yield return null;
    }

    public void DealDamage()
    {
        
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D target in targets)
        {
            Debug.Log("TARGET");
            if (target.CompareTag("Mob"))
            {
                Debug.Log("DEAL DAMAGE");
                target.GetComponent<BaseMob>().TakeDamage(damage);
            }
        }
    }
}
