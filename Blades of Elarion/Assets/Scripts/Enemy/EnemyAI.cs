using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int level;
    public TMP_Text levelText;

    [Header("Detection Ranges")]
    public float followRange = 10f;
    public float attackRange = 2f;

    [Header("Roaming Settings")]
    public float roamRadius = 5f;
    [SerializeField] private float roamInterval = 3f;
    public float roamCooldown;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;

    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private HealthSystem health;

    private Vector3 startPosition;
    private float lastRoamTime;
    private float lastAttackTime;

    private enum EnemyState { Roaming, Chasing, Attacking, Parried, Idle }
    private EnemyState currentState;

    private void Awake()
    {
        health = GetComponent<HealthSystem>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPosition = transform.position;
        roamInterval = Random.Range(2f, 10f);
        attackCooldown = Random.Range(4f, 5f);
    }

    private void Start()
    {
        SetHealth();
    }

    private void Update()
    {
        if (player == null || currentState == EnemyState.Parried) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            SetState(EnemyState.Attacking);
        }
        else if(distanceToPlayer <= attackRange && Time.time < lastAttackTime + attackCooldown)
        {
            // If within attack range but on cooldown, stay in attacking state
            SetState(EnemyState.Idle);
        }
        else if (distanceToPlayer <= followRange)
        {
            SetState(EnemyState.Chasing);
        }
        else
        {
            SetState(EnemyState.Roaming);
        }

        HandleStates();
        UpdateAnimation();
    }

    private void SetHealth()
    {
        level = Random.Range(1, 3);
        levelText.text = level.ToString();
        if (level == 1)
        {
            health.maxHealth = Random.Range(50, 80);
            health.currentHealth = health.maxHealth; 
        }
        if (level == 2)
        {
            health.maxHealth = Random.Range(90, 110);
            health.currentHealth = health.maxHealth;
        }
        if (level == 3)
        {
            health.maxHealth = Random.Range(120, 150);
            health.currentHealth = health.maxHealth;
        }
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
            case EnemyState.Roaming:
                HandleRoaming();
                break;
            case EnemyState.Chasing:
                HandleChasing();
                break;
            case EnemyState.Attacking:
                HandleAttacking();
                break;
            case EnemyState.Idle:
                agent.ResetPath();
                animator.SetFloat("Speed", 0f, 0.3f, Time.deltaTime); // Idle animation
                agent.speed = 0f; // Stop moving
                break;
        }
    }

    private void HandleRoaming()
    {
        if (Time.time >= lastRoamTime + roamInterval || agent.remainingDistance < 0.5f)
        {
            roamCooldown -= Time.deltaTime;
            agent.speed = 0f;
            animator.SetFloat("Speed", 0f, 0.3f, Time.deltaTime); 
            if (roamCooldown <= 0f)
            {
                AfterRoamCooldown();
                roamCooldown = roamInterval; 
            }
        }
    }

    void AfterRoamCooldown()
    {
        Vector3 roamPosition = GetRandomPoint(startPosition, roamRadius);
        agent.SetDestination(roamPosition);
        agent.speed = 2f;
        lastRoamTime = Time.time;
    }

    private void HandleChasing()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
            agent.speed = 3.5f;
        }
    }

    private void HandleAttacking()
    {
        agent.ResetPath();
        // Replace this line:
        // transform.LookAt(player);
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // Keep only horizontal rotation
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }
        animator.SetTrigger("Attack");
        agent.speed = 0f; 
        lastAttackTime = Time.time;
    }

    private void UpdateAnimation()
    {

        if (currentState == EnemyState.Roaming && agent.speed > 0.1f)
            animator.SetFloat("Speed", 1f, 0.3f, Time.deltaTime); // Walking
        else if (currentState == EnemyState.Chasing && agent.speed > 0.1f)
            animator.SetFloat("Speed", 2f, 0.3f, Time.deltaTime); // Running
        else
            animator.SetFloat("Speed", 0f, 0.3f, Time.deltaTime); // Idle
    }

    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        Vector3 destination = center + new Vector3(randomPoint.x, 0f, randomPoint.y);
        NavMesh.SamplePosition(destination, out NavMeshHit hit, radius, NavMesh.AllAreas);
        return hit.position;
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
        agent.speed = 0f; // Stop moving
        animator.SetFloat("Speed", 0f, 0.3f, Time.deltaTime); // Reset speed animation
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
        GetComponentInChildren<WeaponDamageDealer>().StartDealingDamage();
    }

    public void EndDealingDamage()
    {
        GetComponentInChildren<WeaponDamageDealer>().EndDealingDamage();
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }
    }

}
