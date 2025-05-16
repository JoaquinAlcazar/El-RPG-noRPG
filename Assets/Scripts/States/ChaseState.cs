using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyBehaviourFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.currentTarget = enemy.player;
        enemy.agent.SetDestination(enemy.player.position);
        enemy.StartStateTimer(enemy.chaseDuration, new PatrolState(enemy));
    }

    public override void Update()
    {
        if (enemy.player != null)
        {
            enemy.agent.SetDestination(enemy.player.position);

            if (enemy.life <= 50)
            {
                enemy.ChangeState(new FleeState(enemy));
            }
        }
    }
}



