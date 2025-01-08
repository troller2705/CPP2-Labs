using System.Net.Mail;
using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    public enum Type { Patrol, Gather, Sleep }

    [System.Serializable]
    public class PatrolPoint
    {
        public string pointName;
        public Transform point;
        public Type patrolType;
        public float holdTime;
    }

    [Header("Enemy Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Patrol Settings")]
    public PatrolPoint[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Transform player;
    private NavMeshAgent agent;
    private float attackTimer = 0f;

    private Animator animator;

    private enum State { Idle, Patrolling, Chasing, Attacking, Dead }
    private State currentState = State.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints.Length > 0)
        {
            currentState = State.Patrolling;
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[currentPatrolIndex].point.position);
            UpdateAnimation();
        }
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Patrolling:
                HandlePatrolling();
                break;
            case State.Chasing:
                HandleChasing();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
        }

        DetectPlayer();
    }

    void HandleIdle()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            SwitchState(State.Chasing);
        }
    }

    void HandlePatrolling()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].point.position);
        }
    }

    void HandleChasing()
    {
        if (player == null)
        {
            SwitchState(State.Idle);
            return;
        }

        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            SwitchState(State.Attacking);
        }
        else if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            SwitchState(State.Patrolling);
        }
    }

    void HandleAttacking()
    {
        if (player == null)
        {
            SwitchState(State.Idle);
            return;
        }

        transform.LookAt(player);

        if (attackTimer <= 0f)
        {
            animator.SetTrigger("Attack1");
            attackTimer = attackCooldown;
            // Implement attack damage logic here
            Debug.Log("Enemy Attacks the Player!");
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            SwitchState(State.Chasing);
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && currentState != State.Attacking)
        {
            SwitchState(State.Chasing);
        }
    }

    void SwitchState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        UpdateAnimation();

        switch (newState)
        {
            case State.Idle:
                agent.isStopped = true;
                break;
            case State.Patrolling:
                agent.isStopped = false;
                agent.speed = patrolSpeed;
                agent.SetDestination(patrolPoints[currentPatrolIndex].point.position);
                break;
            case State.Chasing:
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                break;
            case State.Attacking:
                agent.isStopped = true;
                break;
        }
    }

    void UpdateAnimation()
    {
        animator.SetBool("WalkForward", currentState == State.Patrolling);
        animator.SetBool("RunForward", currentState == State.Chasing);
        animator.SetBool("Combat Idle", currentState == State.Attacking);
    }

    public void Die()
    {
        currentState = State.Dead;
        animator.SetBool("Death", true);
        agent.isStopped = true;
        Debug.Log("Enemy has died.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
