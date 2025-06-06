using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class WeaponDamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    public bool isHeavyAttack = false;
    public int damageAmount;

    [Header("Detection")]
    public LayerMask targetLayers;
    public string[] targetTags;

    [Header("Optional VFX")]
    public GameObject hitVFX;

    private bool canDealDamage = false;
    private List<GameObject> alreadyHit = new List<GameObject>();

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void StartDealingDamage()
    {
        canDealDamage = true;
        alreadyHit.Clear();
    }

    public void EndDealingDamage()
    {
        canDealDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        if (targetTags.Length > 0 && !TagMatch(other.tag)) return;
        if (alreadyHit.Contains(other.gameObject)) return;

        // 🛡️ BLOCK & PARRY CHECK
        if (other.TryGetComponent<BlockAndParrySystem>(out var blockAndParry))
        {
            if (blockAndParry.IsParryActive())
            {
                Debug.Log("Attack was parried!");
                transform.root.GetComponent<EnemyAI>()?.GetParried(); // Only if attacking enemy
                transform.root.GetComponent<BossAI>()?.GetParried(); // Only if attacking lizard
                alreadyHit.Add(other.gameObject);
                return;
            }
            else if (blockAndParry.IsBlocked())
            {
                Debug.Log("Attack was blocked!");
                blockAndParry.OnBlockedHit(); // Drain stamina, play effects
                alreadyHit.Add(other.gameObject);
                return;
            }
        }

        // ❤️ Apply damage
        if (other.TryGetComponent<HealthSystem>(out var health))
        {
            health.TakeDamage(damageAmount);

            if (hitVFX != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                GameObject hitFX = Instantiate(hitVFX, hitPoint, Quaternion.identity);
                Destroy(hitFX, 1.5f);
            }

            alreadyHit.Add(other.gameObject);
        }
    }

    private bool TagMatch(string otherTag)
    {
        foreach (var tag in targetTags)
        {
            if (otherTag == tag) return true;
        }
        return false;
    }
}
