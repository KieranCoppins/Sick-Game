using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AOE : MonoBehaviour
{
    CircleCollider2D cc;
    int initialDamage;
    int continousDamage;
    float lifespan;
    float damageRate;
    bool friendlyFire;

    bool firstDamage = true;
    bool dealDamage = false;

    float duration;
    float lastDamageDelt;

    protected void Awake()
    {
        cc = GetComponent<CircleCollider2D>();
    }

    public void Initialise(int iD, int cD, float lS, bool fF, float dR)
    {
        initialDamage = iD;
        continousDamage = cD;
        lifespan = lS;
        friendlyFire = fF;
        damageRate = dR;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (firstDamage)
            {
                // deal initial damage to player
            }
            // deal continous damage to player
        }

        if (friendlyFire && collision.CompareTag("Mob"))
        {
            if (firstDamage)
            {
                Debug.Log("Dealing initial damage");
                collision.GetComponent<BaseMob>().TakeDamage(initialDamage);
            }
            if (dealDamage)
            {
                Debug.Log("Dealing continuous damage");
                collision.GetComponent<BaseMob>().TakeDamage(continousDamage);
            }
        }
    }

    protected void FixedUpdate()
    {
        duration += Time.fixedDeltaTime;
        // Make sure our first tick we deal the initial damage
        if (duration > Time.fixedDeltaTime * 2)
        {
            firstDamage = false;
        }
        lastDamageDelt += Time.fixedDeltaTime;
        if (lastDamageDelt >= damageRate)
        {
            lastDamageDelt = 0;
            dealDamage = true;
        }
        else
        {
            dealDamage = false;
        }
        if (duration >= lifespan)
        {
            Destroy(this.gameObject);
        }
    }
}
