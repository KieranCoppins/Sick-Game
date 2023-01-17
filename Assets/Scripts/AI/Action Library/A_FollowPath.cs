using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class A_FollowPath : CustomAction
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _waitTime;
    private int _currentWaypoint = 0;

    public A_FollowPath() : base()
    {
        Flags |= ActionFlags.Interruptable;
    }

    public override IEnumerator Execute()
    {
        while (true)
        {
            float distance = Vector2.Distance(Mob.transform.position, Mob.IdlePathNodes[_currentWaypoint].position);
            while (distance > 0.3f)
            {
                Vector2 dir = Mob.GetMovementVector(Mob.IdlePathNodes[_currentWaypoint].position, true);
                Mob.RigidBody.velocity = dir * _walkSpeed;
                distance = Vector2.Distance(Mob.transform.position, Mob.IdlePathNodes[_currentWaypoint].position);
                yield return null;
            }
            Mob.RigidBody.velocity = Vector2.zero;
            _currentWaypoint++;
            if (_currentWaypoint > Mob.IdlePathNodes.Length - 1)
                _currentWaypoint = 0;
            yield return new WaitForSeconds(_waitTime);
        }
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "The mob will follow along the idle path the mob has.";
    }
}