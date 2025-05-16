using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum state
{
    Damaged,
    Patrol,
    Chase,
    Flee,
    Death
}

public class EnemyBehavior : MonoBehaviour
{
    public float speed;
    public int life;
    public NavMeshAgent agent;
    public state state;

    public Transform player;
    public float visionRange = 10f;
    public float visionAngle = 60f;
    public LayerMask obstacleMask;

    public Transform patrolA;
    public Transform patrolB;
    public Transform currentTarget;

    public int followStateTimer = 5;
    public int fleeStateTimer = 5;

    private Coroutine currentCoroutine;

    void Start()
    {
        currentTarget = patrolA.transform;
        life = 100;
        speed *= 0.01f;
        agent = GetComponent<NavMeshAgent>();
        state = state.Patrol;
        agent.destination = currentTarget.position;
    }

    void Update()
    {
        switch (state)
        {
            case state.Patrol:
                PatrolBehavior();
                break;

            case state.Chase:
                ChaseBehavior();
                break;

            case state.Flee:
                FleeBehavior();
                break;

            case state.Death:
                DeathBehavior();
                return;
        }

        if (CanSeePlayer())
        {
            if (life <= 50 && state != state.Flee)
            {
                state = state.Flee;
                RestartCoroutine(followTimer(fleeStateTimer, state.Patrol));
            }
            else if (state != state.Chase && state != state.Flee)
            {
                state = state.Chase;
                currentTarget = player;
                RestartCoroutine(followTimer(followStateTimer, state.Patrol));
            }
        }



        if (Input.GetKeyDown(KeyCode.Space)) life -= 51;

        if (life <= 0 && state != state.Death)
        {
            state = state.Death;
        }

        // Ejemplo para activar Flee manualmente (puedes usar otra condición)
        if (Input.GetKeyDown(KeyCode.F))
        {
            state = state.Flee;
            RestartCoroutine(followTimer(fleeStateTimer, state.Patrol));
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < visionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer < visionAngle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ChaseBehavior()
    {
        agent.destination = currentTarget.position;
    }

    public void FleeBehavior()
    {
        if (player != null)
        {
            Vector3 directionAway = (transform.position - player.position).normalized;
            Vector3 fleeTarget = transform.position + directionAway * 10f; // Puedes ajustar la distancia
            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeTarget, out hit, 10f, NavMesh.AllAreas))
            {
                agent.destination = hit.position;
            }
        }
    }

    public void PatrolBehavior()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentTarget = (currentTarget == patrolA) ? patrolB : patrolA;
            agent.SetDestination(currentTarget.position);
        }
    }

    public void DeathBehavior()
    {
        Destroy(gameObject);
    }

    private void RestartCoroutine(IEnumerator routine)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(routine);
    }

    public IEnumerator followTimer(float duration, state returnState)
    {
        yield return new WaitForSeconds(duration);

        if (state != state.Death)
        {
            state = returnState;
            currentTarget = patrolA;
            agent.SetDestination(currentTarget.position);
        }
    }
}
