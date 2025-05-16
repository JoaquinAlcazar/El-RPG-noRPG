using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : EnemyState
{
    public DeathState(EnemyBehaviourFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.agent.isStopped = true;
        GameObject.Destroy(enemy.gameObject);
    }

    public override void Update() { }
}


