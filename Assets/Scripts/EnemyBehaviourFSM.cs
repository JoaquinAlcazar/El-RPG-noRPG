using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Rendering;

public class EnemyBehaviourFSM : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform patrolA;
    public Transform patrolB;
    public float visionRange = 10f;
    public float visionAngle = 60f;
    public LayerMask obstacleMask;
    public int life = 100;

    public float chaseDuration = 5f;
    public float fleeDuration = 5f;

    [HideInInspector] public Transform currentTarget;

    private EnemyState currentState;
    private Coroutine stateTimer;

    void Start()
    {
        currentTarget = patrolA;
        agent = GetComponent<NavMeshAgent>();
        ChangeState(new PatrolState(this));
    }

    void Update()
    {
        if (life <= 0 && !(currentState is DeathState))
        {
            ChangeState(new DeathState(this));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) life -= 51;

        currentState?.Update();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState?.Enter();
    }

    public void StartStateTimer(float duration, EnemyState nextState)
    {
        if (stateTimer != null)
            StopCoroutine(stateTimer);

        stateTimer = StartCoroutine(StateTimer(duration, nextState));
    }

    private IEnumerator StateTimer(float duration, EnemyState nextState)
    {
        yield return new WaitForSeconds(duration);

        if (!(currentState is DeathState))
        {
            ChangeState(nextState);
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= visionRange)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle < visionAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distance, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SwitchPatrolPoint()
    {
        currentTarget = (currentTarget == patrolA) ? patrolB : patrolA;
    }

    public void SetDestinationToCurrentPatrolPoint()
    {
        agent.SetDestination(currentTarget.position);
    }
}
