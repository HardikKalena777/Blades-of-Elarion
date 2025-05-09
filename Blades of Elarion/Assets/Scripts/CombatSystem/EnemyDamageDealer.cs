using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    public List<GameObject> hasDealtDamage;

    public float weaponLength;
    public int lightDamage;
    public int heavyDamage;
    public bool canDealDamage;

    public LayerMask enemyLayer;

    CombatManager combatManager;

    private void Awake()
    {
        combatManager = GetComponentInParent<CombatManager>();
    }

    private void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    private void Update()
    {
        HandleWeaponDamage();
    }

    public void HandleWeaponDamage()
    {
        if (canDealDamage)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, weaponLength, enemyLayer))
            {
                if (hit.transform.TryGetComponent<EnemyAI>(out EnemyAI enemy) && !hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    // Damage     
                    //int damage = combatManager.currentState == State.LightAttack ? lightDamage : (combatManager.currentState == State.HeavyAttack ? heavyDamage : 0);
                    enemy.TakeDamage(lightDamage);
                    enemy.PlayHitVFX(hit.point);
                    hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }
    }
    public void StartDealingDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealingDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,transform.position + transform.forward * weaponLength);
    }

}
