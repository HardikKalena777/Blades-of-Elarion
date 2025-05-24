using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    public float[] lightAttackStaminaCost = { 10f, 10f, 15f };
    public float[] unarmedAttackStaminaCost = { 5f, 5f, 8f };
    public float comboResetTime = 1.5f;

    [Header("References")]
    public Animator animator;
    public StaminaSystem staminaSystem;
    public Collider weaponHitbox; // CHANGED FROM Transform to Collider
    public ThirdPersonController movementController;
    public WeaponHandler weaponHandler;
    public WeaponDamageDealer damageDealer;

    public bool isAttacking = false;
    private bool comboQueued = false;
    private int comboStep = 0;
    private float lastAttackTime;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movementController == null)
            movementController = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        //HandleAttackInput();
        ResetComboIfTimedOut();
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

    void StartComboAttack()
    {
        comboStep = comboStep >= 3 ? 0 : comboStep;
        bool usingWeapon = weaponHandler != null && weaponHandler.IsWeaponDrawn;

        float staminaCost = usingWeapon ? lightAttackStaminaCost[comboStep] : unarmedAttackStaminaCost[comboStep];
        if (!staminaSystem.HasStamina(staminaCost))
            return;

        staminaSystem.UseStamina(staminaCost);
        comboStep++;
        isAttacking = true;
        //animator.applyRootMotion = true;
        comboQueued = false;
        lastAttackTime = Time.time;

        string triggerName = usingWeapon ? $"LightAttack{comboStep}" : "UnarmedAttack";
        animator.SetTrigger(triggerName);
    }

    void ResetComboIfTimedOut()
    {
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
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
