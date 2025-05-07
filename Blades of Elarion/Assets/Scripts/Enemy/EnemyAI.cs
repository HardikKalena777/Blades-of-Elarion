using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public bool IsAlive;
    public EnemySO enemyData;
    public string[] attackAnimations;
    public float attackRange;
    public float followRange;
    public GameObject hitVFX;
    public GameObject ragdoll;
    public Slider healthBar;

    GameObject player;
    Animator animator;
    NavMeshAgent agent;

    [SerializeField] private int currentHealth;
    int maxHealth;

    [Header("Combat")]
    float attackCooldown;
    float newDestinationCooldown;
    float timePassed;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        maxHealth = enemyData.maxHealth;
        currentHealth = maxHealth;
        attackCooldown = enemyData.attackCooldown;
        newDestinationCooldown = enemyData.newDestinationCooldown;
    }

    private void Update()
    {
        if(IsAlive)
        {
            AIBehaviour();
        }
    }

    public void AIBehaviour()
    {
        if (player == null)
            return;
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

        UpdateHealthUI(currentHealth,healthBar);
        animator.SetTrigger("Hit");
        CameraShake.Instance.ShakeCamera(1f, 0.2f);

        HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f); // Trigger haptic feedback on hit

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(ragdoll, transform.position, transform.rotation);
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
            Destroy(vfx, 1f); // Destroy the VFX after 2 seconds
        }
    }

    private void UpdateHealthUI(int health, Slider healthBar)
    {
        if (healthBar != null)
        {
            healthBar.value = (float)health / maxHealth; // Correctly update the health bar value as a percentage
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
