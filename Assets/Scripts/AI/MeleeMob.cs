using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMob : BaseMob
{
    public float MeleeRange { get { return _meleeRange; } protected set { _meleeRange = value; } }
    public float MeleeDamage { get { return _meleeDamage; } protected set { _meleeDamage = value; } }
    public float MeleeSpeed { get { return _meleeSpeed; } protected set { _meleeSpeed = value; } }

    [Header("Melee Mob Attributes")]
    [SerializeField] float _meleeRange;
    [SerializeField] float _meleeDamage;
    [SerializeField] float _meleeSpeed;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override float MoveAround(Vector2 targetDir, Vector2 dir, Vector2 target, bool moveStraight)
    {
        float dist = Vector2.Distance(target, transform.position);

        if (moveStraight)
            return Vector2.Dot(targetDir, dir);

        if (dist < 2f)
            return Vector2.Dot(targetDir, dir) + Vector2.Dot(rb.velocity.normalized, dir);

        return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir) - 0.9f) + Vector2.Dot(rb.velocity.normalized, dir);

    }
}
