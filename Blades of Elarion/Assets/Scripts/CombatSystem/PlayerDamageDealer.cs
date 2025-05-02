using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageDealer : MonoBehaviour
{
    public WeaponSO weaponData;
    public float weaponLength;

    LayerMask enemyLayer;
    int damage;
    bool hasDealtDamage;
    bool canDealDamage;


    private void Start()
    {
        canDealDamage = false;
        hasDealtDamage = false;
        damage = weaponData.lightDamage;
        enemyLayer = weaponData.enemyLayer;
    }

    private void Update()
    {
        HandleWeaponDamage();
    }

    public void HandleWeaponDamage()
    {
        if (canDealDamage && !hasDealtDamage)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, weaponLength, enemyLayer))
            {
                if (hit.transform.TryGetComponent<HealthSystem>(out HealthSystem playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Enemy has dealt damage");
                    playerHealth.PlayHitVFX(hit.point);
                    hasDealtDamage = true;
                }
            }
        }
    }
    public void StartDealingDamage()
    {
        canDealDamage = true;
        hasDealtDamage = false;
    }

    public void EndDealingDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * weaponLength);
    }
}
