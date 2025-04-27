using UnityEngine;

[CreateAssetMenu(fileName = "AttackSO", menuName = "ScriptableObjects/AttackSO")]
public class AttackSO : ScriptableObject
{
    public AnimatorOverrideController animatorOV;
    public float damage;
}
