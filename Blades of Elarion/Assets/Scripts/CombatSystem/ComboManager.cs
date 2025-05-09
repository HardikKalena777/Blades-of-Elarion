using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class ComboManager : MonoBehaviour
{
    ThirdPersonController thirdPersonController;
    WeaponManager weaponManager;

    [SerializeField] private Animator animator;
    [SerializeField] private float comboDelay = 1.2f;

    private int numberOfClicks = 0;
    private float lastClickTime = 0f;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        weaponManager = GetComponent<WeaponManager>();
    }
    private void Update()
    {
        HandleAttackAnimationTime();
    }
    public void HandleAttackAnimationTime()
    {
        if(Time.time - lastClickTime > comboDelay)
        {
            numberOfClicks = 0;
        }
    }

    public void Attack()
    {
        lastClickTime = Time.time;
        numberOfClicks++;

        if(numberOfClicks >= 1)
        {
            animator.SetTrigger("Attack1");
            thirdPersonController.DisableMovement();
        }

        numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);
    }

    public void ComboAttack1Transition()
    {
        if (numberOfClicks >= 2)
        {
            animator.SetTrigger("Attack2");
            thirdPersonController.DisableMovement();
        }
    }
    public void ComboAttack2Transition()
    {
        if (numberOfClicks >= 3)
        {
            animator.SetTrigger("Attack3");
            thirdPersonController.DisableMovement();
        }
    }

    public void ResetAttackAnimation()
    {
        thirdPersonController.EnableMovement();

        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");

        numberOfClicks = 0;
        lastClickTime = 0f;
    }
}
