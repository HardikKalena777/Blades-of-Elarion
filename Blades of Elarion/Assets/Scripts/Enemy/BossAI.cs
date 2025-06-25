using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    public WeaponDamageDealer weaponDamageDealerLeft;
    public WeaponDamageDealer weaponDamageDealerRight;
    public EnemySO enemyData;

    [Header("Detection Ranges")]
    public float followRange;
    public float attackRange;

    [Header("Roaming Settings")]
    public float roamRadius;
    public float roamInterval;

    [Header("Attack Settings")]
    public float attackCooldown;

    [Header("Events")]
    public UnityEvent onBattleStart;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 startPosition;
    private float lastRoamTime;
    private float lastAttackTime;

    private bool battleStarted = false;

    private enum EnemyState { Chasing, Attacking, Parried, Idle }
    private EnemyState currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPosition = transform.position;
    }

    private void Start()
    {
        followRange = enemyData.followRange;

        roamRadius = enemyData.roamRadius;
        roamInterval = enemyData.roamInterval;

        attackRange = enemyData.attackRange;
        attackCooldown = enemyData.attackCooldown;

        GetComponent<HealthSystem>().maxHealth = enemyData.maxHealth;
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
        else if(distanceToPlayer > followRange)
        {
            SetState(EnemyState.Idle);
        }

        if ((currentState == EnemyState.Chasing || currentState == EnemyState.Attacking) && player != null)
            transform.LookAt(player);

        HandleStates();
        UpdateAnimation();
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case EnemyState.Attacking:
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                agent.ResetPath();
                agent.speed = 0f;
                break;
            case EnemyState.Parried:
                GetParried();
                agent.ResetPath();
                break;
            // Add other state transitions as needed
        }
    }

    private void HandleStates()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                agent.ResetPath();
                animator.SetFloat("Speed", 0f, 0.3f, Time.deltaTime); // Idle
                break;
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
            if (!battleStarted)
            {
                onBattleStart?.Invoke();
                battleStarted = true;
            }
            agent.SetDestination(player.position);
            agent.speed = 2.5f;
        }
    }

    private void HandleAttacking()
    {
        agent.ResetPath();
        agent.speed = 0f;
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
        CameraShake.Instance.ShakeCamera(1f, 0.2f);
        HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f);
        Invoke(nameof(RecoverFromParry), 2f); // Recover after 2 seconds
    }

    private void RecoverFromParry()
    {
        SetState(EnemyState.Attacking);
    }

    public void StartDealingDamageLeft()
    {
        weaponDamageDealerLeft.StartDealingDamage();
    }

    public void EndDealingDamageLeft()
    {
        weaponDamageDealerLeft.EndDealingDamage();
    }

    public void StartDealingDamageRight()
    {
        weaponDamageDealerRight.StartDealingDamage();
    }

    public void EndDealingDamageRight()
    {
        weaponDamageDealerRight.EndDealingDamage();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }

}
