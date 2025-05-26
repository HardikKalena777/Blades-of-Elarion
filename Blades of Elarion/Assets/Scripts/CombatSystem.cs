using StarterAssets;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    public float[] lightAttackStaminaCost = { 10f, 10f, 15f };
    public float[] unarmedAttackStaminaCost = { 5f, 5f, 8f };
    public float comboResetTime = 1.5f;
    public TMP_Text comboText;

    [Header("References")]
    public Animator animator;
    public StaminaSystem staminaSystem;
    public Collider weaponHitbox; // CHANGED FROM Transform to Collider
    public ThirdPersonController movementController;
    public WeaponHandler weaponHandler;
    public WeaponDamageDealer damageDealer;
    public BlockAndParrySystem blockParry;

    public bool isAttacking = false;
    private bool comboQueued = false;
    public int comboStep = 0;
    private float lastAttackTime;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movementController == null)
            movementController = GetComponent<ThirdPersonController>();
        if(blockParry == null)
            blockParry = GetComponent<BlockAndParrySystem>();
    }

    void Update()
    {
        HandleUI();
        ResetComboIfTimedOut();
        if(isAttacking)
        {
            animator.SetLayerWeight(2, 0f);
        }
        else
        {
            animator.SetLayerWeight(2, 1f);
        }
    }

    public void HandleAttackInput()
    {
        //if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.JoystickButton2)) // Gamepad West button
        //{
            if (isAttacking)
            {
                comboQueued = true;
            }
            else
            {
                StartComboAttack();
            }
        //}
    }

    void HandleUI()
    {
        if(comboStep > 0)
        {
            comboText.text = comboStep + "X"; 
        }
        else
        {
            comboText.text = string.Empty;
        }
    }

    void StartComboAttack()
    {
        bool usingWeapon = weaponHandler != null && weaponHandler.IsWeaponDrawn;
        if (blockParry.IsBlocked())
            return;
        if (!usingWeapon)
            return;
        movementController.DisableMovement();

        comboStep = comboStep >= 3 ? 0 : comboStep;

        float staminaCost = lightAttackStaminaCost[comboStep];
        if (!staminaSystem.HasStamina(staminaCost))
            return;

        staminaSystem.UseStamina(staminaCost);
        comboStep++;
        isAttacking = true;
        //animator.applyRootMotion = true;
        comboQueued = false;
        lastAttackTime = Time.time;

        string triggerName = $"LightAttack{comboStep}";
        animator.SetTrigger(triggerName);
    }

    void ResetComboIfTimedOut()
    {
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
            movementController.EnableMovement();
        }
    }

    // Animation Events
    public void OnComboWindow()
    {
        if (comboQueued)
        {
            StartComboAttack();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void EnableWeaponHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.enabled = true;
        damageDealer.StartDealingDamage();
    }

    public void DisableWeaponHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.enabled = false;
        damageDealer.EndDealingDamage();
    }

    public void StartRootMotionAttack()
    {
        isAttacking = true;
    }
}
