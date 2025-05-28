using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LizardAI : MonoBehaviour
{
    public enum State { Idle, Chasing, Attacking, Parried }
    public State currentState = State.Idle;

    [Header("References")]
    public Transform player;

    [Header("Settings")]
    public float chaseRange = 12f;
    public float attackRange = 2.5f;
    public float moveSpeed = 2.5f;
    public float attackCooldown = 2f;

    private Animator animator;
    private float lastAttackTime;
    private float currentSpeed = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        if (player == null || currentState == State.Parried) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= chaseRange)
                    SetState(State.Chasing);
                break;

            case State.Chasing:
                if (distanceToPlayer <= attackRange)
                    SetState(State.Attacking);
                else
                    ChasePlayer();
                break;

            case State.Attacking:
                if (distanceToPlayer > attackRange)
                    SetState(State.Chasing);
                else if (Time.time >= lastAttackTime + attackCooldown)
                    AttackPlayer();
                break;
        }

        UpdateAnimation();
    }

    private void SetState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    private void ChasePlayer()
    {
        if(currentState == State.Attacking) return;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        transform.position += dir * currentSpeed * Time.deltaTime;
        currentSpeed = moveSpeed;
    }

    private void AttackPlayer()
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
        currentSpeed = 0f;
    }

    private void UpdateAnimation()
    {
        if (currentState == State.Chasing)
            animator.SetFloat("Speed", 2f, 0.3f, Time.deltaTime);
        else if (currentState == State.Attacking || currentState == State.Idle)
            animator.SetFloat("Speed", 0f, 0.3f, Time.deltaTime);
    }

    public void GetParried()
    {
        if (currentState == State.Parried) return;

        SetState(State.Parried);
        animator.SetTrigger("Parried");
        CancelInvoke(nameof(RecoverFromParry));
        Invoke(nameof(RecoverFromParry), 2f);
    }

    private void RecoverFromParry()
    {
        SetState(State.Idle);
    }

    public void StartDealingDamage()
    {
        GetComponentInChildren<WeaponDamageDealer>()?.StartDealingDamage();
    }

    public void EndDealingDamage()
    {
        GetComponentInChildren<WeaponDamageDealer>()?.EndDealingDamage();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
