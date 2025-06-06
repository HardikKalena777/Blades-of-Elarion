using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    public WeaponDamageDealer weaponDamageDealerLeft;
    public WeaponDamageDealer weaponDamageDealerRight;

    [Header("Detection Ranges")]
    public float followRange = 10f;
    public float attackRange = 2f;

    [Header("Roaming Settings")]
    public float roamRadius = 5f;
    public float roamInterval = 3f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 startPosition;
    private float lastRoamTime;
    private float lastAttackTime;

    private enum EnemyState { Chasing, Attacking, Parried }
    private EnemyState currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (player == null || currentState == EnemyState.Parried) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            SetState(EnemyState.Attacking);
        }
        else if (distanceToPlayer <= followRange)
        {
            SetState(EnemyState.Chasing);
        }
        transform.LookAt(player);

        HandleStates();
        UpdateAnimation();
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    private void HandleStates()
    {
        switch (currentState)
        {
            case EnemyState.Chasing:
                HandleChasing();
                break;
            case EnemyState.Attacking:
                HandleAttacking();
                break;
        }
    }


    private void HandleChasing()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
            agent.speed = 2.5f;
        }
    }

    private void HandleAttacking()
    {
        agent.ResetPath();
        //Vector3 direction = (player.position - transform.position).normalized;
        //direction.y = 0f; // Keep only horizontal rotation
        //if (direction.sqrMagnitude > 0.001f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(direction);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        //}
        animator.SetTrigger("Attack");
        agent.speed = 0f;
        lastAttackTime = Time.time;
    }

    private void UpdateAnimation()
    {
        float speed = agent.velocity.magnitude;

        if (currentState == EnemyState.Chasing && speed > 0.1f)
            animator.SetFloat("Speed", 2f, 0.3f, Time.deltaTime); // Running
        else
            animator.SetFloat("Speed", 0f, 0.3f, Time.deltaTime); // Idle
    }

    public void ForceAggro(Transform target)
    {
        player = target;
        SetState(EnemyState.Chasing);
    }

    public void GetParried()
    {
        if (currentState == EnemyState.Parried) return;

        SetState(EnemyState.Parried);
        agent.ResetPath();
        animator.SetTrigger("Parried");
        CameraShake.Instance.ShakeCamera(1f, 0.2f);
        HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f);
        Invoke(nameof(RecoverFromParry), 2f); // Recover after 2 seconds
    }

    private void RecoverFromParry()
    {
        SetState(EnemyState.Attacking);
    }

    public void StartDealingDamage()
    {
        weaponDamageDealerLeft.StartDealingDamage();
        weaponDamageDealerRight.StartDealingDamage();
    }

    public void EndDealingDamage()
    {
        weaponDamageDealerLeft.EndDealingDamage();
        weaponDamageDealerRight.EndDealingDamage();
    }

}
