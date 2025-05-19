using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    public List<GameObject> hasDealtDamage;

    public float weaponLength;
    public float weaponWidth;
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
            Vector3 start = transform.position;
            Vector3 end = transform.position + transform.forward * weaponLength;
            float radius = weaponWidth;

            RaycastHit[] hits = Physics.CapsuleCastAll(start, end, radius, transform.forward, 0f, enemyLayer);

            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<EnemyAI>(out EnemyAI enemy) && !hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    enemy.TakeDamage(lightDamage);
                    enemy.PlayHitVFX(start);
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
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(weaponWidth, weaponWidth, weaponLength));
    }
}
