using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AOE : MonoBehaviour
{
    private int _initialDamage;

    private float _lifespan;

    private int _damageRate;

    private bool _friendlyFire;

    private BaseCharacter _caster;

    private List<Collider2D> _colliders;

    public void Initialise(int initialDamage, float lifespan, bool friendlyFire, int damageRate, BaseCharacter caster)
    {
        _initialDamage = initialDamage;
        _lifespan = lifespan;
        _friendlyFire = friendlyFire;
        _damageRate = damageRate;
        _caster = caster;
        _colliders = new List<Collider2D>();
        StartCoroutine(DealDamageOnTick());
        StartCoroutine(DestroyHandler());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _colliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _colliders.Remove(collision);
    }

    private void DealDamage(int dmg)
    {
        foreach (Collider2D collider in _colliders)
        {
            if (_friendlyFire && collider.CompareTag("Mob"))
            {
                collider.GetComponent<BaseMob>().TakeDamage(_caster, dmg);
            }
            else if (collider.CompareTag("Player"))
            {
                // Deal dmg to player
            }
        }
    }

    private IEnumerator DealDamageOnTick()
    {
        yield return new WaitForSeconds(0.2f); // Wait for 200 milliseconds to get all colliders to deal initial damage to
        DealDamage(_initialDamage);
        yield return new WaitForSeconds(1); // Wait a second then we deal our damage rate

        while (true)
        {
            DealDamage(_damageRate);
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator DestroyHandler()
    {
        yield return new WaitForSeconds(_lifespan);  // After our life span is up we can destroy ourselves
        Destroy(gameObject);
    }
}
