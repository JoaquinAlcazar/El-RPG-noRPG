using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeState : EnemyState
{
    public FleeState(EnemyBehaviourFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        Vector3 dirAway = (enemy.transform.position - enemy.player.position).normalized;
        Vector3 fleeTarget = enemy.transform.position + dirAway * 10f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, 10f, NavMesh.AllAreas))
        {
            enemy.agent.SetDestination(hit.position);
        }

        enemy.StartStateTimer(enemy.fleeDuration, new PatrolState(enemy));
    }
}


