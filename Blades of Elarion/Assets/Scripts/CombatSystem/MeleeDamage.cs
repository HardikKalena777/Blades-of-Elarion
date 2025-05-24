using Cysharp.Threading.Tasks;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    public int damageAmount;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                //enemy.TakeDamage(damageAmount);
            }
        }
    }
}
