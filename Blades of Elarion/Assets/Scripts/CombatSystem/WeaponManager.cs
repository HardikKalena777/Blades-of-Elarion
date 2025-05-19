using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    EnemyDamageDealer damageDealer;
    public InputActionAsset playerInput;
    public InputAction block;

    [Header("Combat Variables")]
    public bool canDrawWeapon = true;
    public bool blocking;

    Animator animator;

    [Header("Weapons")]
    public Transform weaponHolder;
    public Transform weaponTransform;
    public Transform weaponSheathTransform;

    [Header("Weapon Events")]
    public UnityEvent onDrawWeapon;
    public UnityEvent onSheathWeapon;

    public UnityEvent onBlock;
    public UnityEvent onUnblock;

    private void Awake()
    {
        damageDealer = GetComponentInChildren<EnemyDamageDealer>();

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerInput.FindAction(block.ToString()).performed += ctx => HandleBlock();
        playerInput.FindAction(block.ToString()).canceled += ctx => HandleUnblock();
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

    public void HandleBlock()
    {
        animator.SetBool("Blocked", true);
        blocking = true;
        onBlock?.Invoke();
    }

    public void HandleUnblock()
    {
        animator.SetBool("Blocked", false);
        blocking = false;
        onUnblock?.Invoke();
    }

    private void HandleWeaponSheath()
    {
        if (!canDrawWeapon)
        {
            canDrawWeapon = true;
            onSheathWeapon?.Invoke();
            animator.SetTrigger("Sheath");
            PlayTargetAnimation("Locomotion", 0.1f);
        }
    }

    private void HandleWeaponDraw()
    {
        if (canDrawWeapon)
        {
            canDrawWeapon = false;
            onDrawWeapon?.Invoke();
            animator.SetTrigger("Draw");
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

    public void OnStartDealingDamage()
    {
        damageDealer.StartDealingDamage();
    }

    public void OnEndDealingDamage()
    {
        damageDealer.EndDealingDamage();
    }
    #endregion
}
