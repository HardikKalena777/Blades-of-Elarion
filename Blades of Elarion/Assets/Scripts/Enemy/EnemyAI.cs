using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    public EnemySO enemyData;
    public string[] attackAnimations;
    public float attackRange;
    public float followRange;
    public GameObject hitVFX;

    GameObject player;
    Animator animator;
    NavMeshAgent agent;

    [SerializeField] private int currentHealth;
    int maxHealth;

    [Header("Combat")]
    float attackCooldown;
    float attackDamage;
    float newDestinationCooldown;
    float timePassed;

    PlayerDamageDealer damageDealer;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        maxHealth = enemyData.maxHealth;
        currentHealth = maxHealth;
        attackCooldown = enemyData.attackCooldown;
        attackDamage = enemyData.attackDamage;
        newDestinationCooldown = enemyData.newDestinationCooldown;
    }

    private void Update()
    {
        AIBehaviour();
    }

    public void AIBehaviour()
    {
        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);

        if (timePassed >= attackCooldown)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                PlayTargetAnimation(attackAnimations[UnityEngine.Random.Range(0, attackAnimations.Length)], 0);
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;

        if(newDestinationCooldown <= 0 && Vector3.Distance(player.transform.position, transform.position) <= followRange)
        {
            newDestinationCooldown = enemyData.newDestinationCooldown;
            agent.SetDestination(player.transform.position);
        }
        newDestinationCooldown -= Time.deltaTime;
        transform.LookAt(player.transform);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hit");

        HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f); // Trigger haptic feedback on hit

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }

    public void PlayTargetAnimation(string TargetAnimation, float transitionDuration)
    {
        animator.CrossFade(TargetAnimation, transitionDuration);
    }

    public void StartDealingDamage()
    {
        GetComponentInChildren<PlayerDamageDealer>().StartDealingDamage();
    }

    public void EndDealingDamage()
    {
        GetComponentInChildren<PlayerDamageDealer>().EndDealingDamage();
    }

    public void PlayHitVFX(Vector3 hitPosition)
    {
        if (hitVFX != null)
        {
            GameObject vfx = Instantiate(hitVFX, hitPosition, Quaternion.identity);
            Destroy(vfx, 3f); // Destroy the VFX after 2 seconds
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}
