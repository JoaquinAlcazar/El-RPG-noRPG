using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyState
{
    public PatrolState(EnemyBehaviourFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetDestinationToCurrentPatrolPoint();
    }

    public override void Update()
    {
        if (enemy.CanSeePlayer())
        {
            if (enemy.life <= 50)
                enemy.ChangeState(new FleeState(enemy));
            else
                enemy.ChangeState(new ChaseState(enemy));

            return;
        }

        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.5f)
        {
            enemy.SwitchPatrolPoint();
            enemy.SetDestinationToCurrentPatrolPoint();
        }
    }
}


