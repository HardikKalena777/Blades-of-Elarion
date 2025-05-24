using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public bool IsAlive;
    public EnemySO enemyData;
    public string[] attackAnimations;
    public float attackRange;
    public float followRange;
    //public GameObject hitVFX;
    //public GameObject ragdoll;
    //public Slider healthBar;
    public bool beingHit;

    GameObject player;
    Animator animator;
    NavMeshAgent agent;

    //private BlockAndParrySystem playerBlockSystem;

    //[SerializeField] private int currentHealth;
    //int maxHealth;

    [Header("Combat")]
    float attackCooldown;
    float newDestinationCooldown;
    float timePassed;
    bool detected;

    private Vector3 roamTarget;
    public float roamRadius = 5f;
    public float roamCooldown = 0f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        //playerBlockSystem = player.GetComponent<BlockAndParrySystem>();

        //maxHealth = enemyData.maxHealth;
        //currentHealth = maxHealth;
        attackCooldown = enemyData.attackCooldown;
        newDestinationCooldown = enemyData.newDestinationCooldown;
    }

    private void Update()
    {
        if (IsAlive)
        {
            AIBehaviour();
        }
    }

    public void AIBehaviour()
    {
        if (player == null) return;

        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer > followRange)
        {
            Roam();
            detected = false;
            return;
        }

        if (timePassed >= attackCooldown && distanceToPlayer <= attackRange)
        {
            if (beingHit) return;

            PlayTargetAnimation(attackAnimations[UnityEngine.Random.Range(0, attackAnimations.Length)], 0);
            timePassed = 0;
        }

        timePassed += Time.deltaTime;

        if (newDestinationCooldown <= 0 && distanceToPlayer <= followRange)
        {
            newDestinationCooldown = enemyData.newDestinationCooldown;
            agent.SetDestination(player.transform.position);
            detected = true;
        }
        newDestinationCooldown -= Time.deltaTime;

        if (detected)
        {
            transform.LookAt(player.transform);
        }
    }

    private void Roam()
    {
        roamCooldown -= Time.deltaTime;
        if (roamCooldown <= 0f || Vector3.Distance(transform.position, roamTarget) < 1f)
        {
            roamTarget = GetRandomRoamPosition();
            agent.SetDestination(roamTarget);
        }
    }

    private Vector3 GetRandomRoamPosition()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * roamRadius;
        Vector3 randomPos = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    //public async void TakeDamage(int damage)
    //{
    //    if (playerBlockSystem != null)
    //    {
    //        if (playerBlockSystem.IsParrying)
    //        {
    //            Debug.Log("Enemy attack parried!");
    //            await GetParried();
    //            return;
    //        }
    //        else if (playerBlockSystem.IsBlocking)
    //        {
    //            Debug.Log("Enemy attack blocked!");
    //            CameraShake.Instance.ShakeCamera(1f, 0.2f);
    //            return;
    //        }
    //    }

    //    currentHealth -= damage;

    //    UpdateHealthUI(currentHealth, healthBar);
    //    animator.SetTrigger("Hit");
    //    CameraShake.Instance.ShakeCamera(1f, 0.2f);
    //    HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f);
    //    beingHit = true;
    //    await UniTask.Delay(500);
    //    beingHit = false;

    //    if (currentHealth <= 0)
    //    {
    //        Die();
    //    }
    //}

    public async UniTask GetParried()
    {
        animator.SetTrigger("Parried");
        CameraShake.Instance.ShakeCamera(1f, 0.2f);
        HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f);
        beingHit = true;
        agent.isStopped = true;
        await UniTask.Delay(2000);

        beingHit = false;
        agent.isStopped = false;
    }

    //private void Die()
    //{
    //    Instantiate(ragdoll, transform.position, transform.rotation);
    //    Destroy(gameObject);
    //}

    public void PlayTargetAnimation(string TargetAnimation, float transitionDuration)
    {
        animator.CrossFade(TargetAnimation, transitionDuration);
    }

    public void StartDealingDamage()
    {
        GetComponentInChildren<WeaponDamageDealer>().StartDealingDamage();
    }

    public void EndDealingDamage()
    {
        GetComponentInChildren<WeaponDamageDealer>().EndDealingDamage();
    }

    //public void PlayHitVFX(Vector3 hitPosition)
    //{
    //    if (hitVFX != null)
    //    {
    //        GameObject vfx = Instantiate(hitVFX, hitPosition, Quaternion.identity);
    //        Destroy(vfx, 1f);
    //    }
    //}

    //private void UpdateHealthUI(int health, Slider healthBar)
    //{
    //    if (healthBar != null)
    //    {
    //        healthBar.value = (float)health / maxHealth;
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, roamRadius);
    }
}
