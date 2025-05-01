using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public int maxHealth;

    [Header("Combat")]  
    public float attackCooldown;
    public float attackDamage;
    public float newDestinationCooldown;
}
