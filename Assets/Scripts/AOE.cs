using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AOE : MonoBehaviour
{
    int initialDamage;
    float lifespan;
    int damageRate;
    bool friendlyFire;

    List<Collider2D> colliders;
    public void Initialise(int iD, float lS, bool fF, int dR)
    {
        initialDamage = iD;
        lifespan = lS;
        friendlyFire = fF;
        damageRate = dR;
        colliders = new List<Collider2D>();
        StartCoroutine(DealDamageOnTick());
        StartCoroutine(DestroyHandler());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        colliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        colliders.Remove(collision);
    }

    void DealDamage(int dmg)
    {
        foreach (Collider2D collider in colliders)
        {
            if (friendlyFire && collider.CompareTag("Mob"))
            {
                collider.GetComponent<BaseMob>().TakeDamage(dmg);
            }
            else if (collider.CompareTag("Player"))
            {
                // Deal dmg to player
            }
        }
    }

    IEnumerator DealDamageOnTick()
    {
        yield return new WaitForSeconds(0.2f); // Wait for 200 milliseconds to get all colliders to deal initial damage to
        DealDamage(initialDamage);
        yield return new WaitForSeconds(1); // Wait a second then we deal our damage rate

        while (true)
        {
            DealDamage(damageRate);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator DestroyHandler()
    {
        yield return new WaitForSeconds(lifespan);  // After our life span is up we can destroy ourselves
        Destroy(gameObject);
    }
}
