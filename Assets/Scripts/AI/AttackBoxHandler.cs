using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoxHandler : TriggerHandler
{
    BaseMob mob;

    private void Awake()
    {
        mob = GetComponentInParent<BaseMob>();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {

    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        mob.ResumeMoving();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
        if (collision.tag != "Player")
            return;

        mob.Attack(collision.gameObject);
    }
}
