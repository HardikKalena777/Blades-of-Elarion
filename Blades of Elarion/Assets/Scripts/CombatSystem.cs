using StarterAssets;
using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    public float[] lightAttackStaminaCost = { 10f, 10f, 15f };
    public float comboResetTime = 1.5f;
    public TMP_Text comboText;

    [Header("References")]
    public Animator animator;
    public StaminaSystem staminaSystem;
    public Collider weaponHitbox;
    public ThirdPersonController movementController;
    public WeaponHandler weaponHandler;
    public WeaponDamageDealer damageDealer;
    public BlockAndParrySystem blockParry;

    private int comboStep = 0;
    private bool isAttacking = false;
    private bool inputBuffered = false;
    private float lastAttackTime = 0f;
    private Coroutine comboResetCoroutine;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (movementController == null)
            movementController = GetComponent<ThirdPersonController>();
        if (blockParry == null)
            blockParry = GetComponent<BlockAndParrySystem>();
    }

    void Update()
    {
        //HandleUI();
        // Reset combo if too much time has passed since last attack
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    public void HandleAttackInput()
    {
        bool usingWeapon = weaponHandler != null && weaponHandler.IsWeaponDrawn;
        if(!usingWeapon)
            return; 
        if (blockParry != null && blockParry.IsBlocked())
            return;

        if (!isAttacking)
        {
            // Start first attack
            comboStep = 0;
            TryAttack();
        }
        else
        {
            // Buffer input for next combo step
            inputBuffered = true;
        }
    }

    private void TryAttack()
    {
        // Check stamina
        if (comboStep >= lightAttackStaminaCost.Length)
            comboStep = 0;

        float staminaCost = lightAttackStaminaCost[comboStep];
        if (!staminaSystem.HasStamina(staminaCost))
            return;

        staminaSystem.UseStamina(staminaCost);
        isAttacking = true;
        lastAttackTime = Time.time;

        // Play animation
        string triggerName = $"LightAttack{comboStep + 1}";
        animator.SetTrigger(triggerName);

        movementController.DisableMovement();

        // Start/reset combo reset timer
        if (comboResetCoroutine != null)
            StopCoroutine(comboResetCoroutine);
        comboResetCoroutine = StartCoroutine(ComboResetTimer());
    }

    // Called by animation event at the end of each attack animation
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        movementController.EnableMovement();

        if (inputBuffered && comboStep < lightAttackStaminaCost.Length - 1)
        {
            comboStep++;
            inputBuffered = false;
            TryAttack();
        }
        else
        {
            ResetCombo();
        }
    }

    private IEnumerator ComboResetTimer()
    {
        yield return new WaitForSeconds(comboResetTime);
        ResetCombo();
    }

    private void ResetCombo()
    {
        comboStep = 0;
        isAttacking = false;
        inputBuffered = false;
        if (comboText != null)
            comboText.text = string.Empty;
    }

    //private void HandleUI()
    //{
    //    if (comboText != null)
    //    {
    //        comboText.text = isAttacking ? $"{comboStep + 1}X" : string.Empty;
    //    }
    //}

    // Animation Events
    public void EnableWeaponHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.enabled = true;
        if (damageDealer != null)
            damageDealer.StartDealingDamage();
    }

    public void DisableWeaponHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.enabled = false;
        if (damageDealer != null)
            damageDealer.EndDealingDamage();
    }
}
