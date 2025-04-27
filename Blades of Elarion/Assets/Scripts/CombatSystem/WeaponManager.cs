using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    AnimationManager animationManager;
    CombatManager combatManager;

    [Header("Combat Variables")]
    private bool canDrawWeapon = true;

    Animator animator;

    [Header("Weapons")]
    public Transform weaponHolder;
    public Transform weaponTransform;
    public Transform weaponSheathTransform;

    [Header("Weapon Events")]
    public UnityEvent onDrawWeapon;
    public UnityEvent onSheathWeapon;

    private void Awake()
    {
        animationManager = GetComponentInParent<AnimationManager>();
        combatManager = GetComponentInParent<CombatManager>();

        animator = GetComponent<Animator>();
    }

    public void HandleWeaponToggle()
    {
        if (canDrawWeapon)
        {
            HandleWeaponDraw();
        }
        else
        {
            HandleWeaponSheath();
        }
    }

    private void HandleWeaponSheath()
    {
        if (!canDrawWeapon)
        {
            canDrawWeapon = true;
            onSheathWeapon?.Invoke();
            animator.SetTrigger("Sheath");
            PlayTargetAnimation("Locomotion", 0.1f);
            combatManager.currentState = State.Base;
        }
    }

    private void HandleWeaponDraw()
    {
        if (canDrawWeapon)
        {
            canDrawWeapon = false;
            onDrawWeapon?.Invoke();
            animator.SetTrigger("Draw");
            combatManager.currentState = State.Combat;
        }
    }

    public void PlayTargetAnimation(string TargetAnimation, float transitionDuration)
    {
        animator.CrossFade(TargetAnimation, transitionDuration);
    }

    private void HandleWeaponParent(Transform parent, Transform child)
    {
        child.SetParent(parent);
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
    }

    #region Animation Events

    public void OnWeaponDraw()
    {
        HandleWeaponParent(weaponHolder, weaponTransform);
    }

    public void OnWeaponSheath()
    {
        HandleWeaponParent(weaponSheathTransform, weaponTransform);
    }
    #endregion
}
