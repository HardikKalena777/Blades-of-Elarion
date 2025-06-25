using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public int maxHealth;

    [Header("Combat")]  
    public float attackCooldown;
    public float attackRange;
    public float followRange;
    public float roamRadius;
    public float roamInterval;
}
