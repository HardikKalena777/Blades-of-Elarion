using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO",menuName = "ScriptableObjects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public float weaponLength;
    public int lightDamage;
    public int heavyDamage;

    public LayerMask enemyLayer;
}
