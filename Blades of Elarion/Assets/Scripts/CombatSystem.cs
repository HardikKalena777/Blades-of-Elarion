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
    public bool isAttacking = false;
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
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
        HandleUI();
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
            StartCoroutine(ComboTextAnimation()); // Show combo text animation
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

    private void HandleUI()
    {
        if(isAttacking)
        {
            StartCoroutine(ComboTextAnimation());
        }
        else
        {
            if (comboText != null)
                comboText.text = string.Empty;
        }
    }

    IEnumerator ComboTextAnimation()
    {
        if (comboText != null)
        {
            float startSize = 150f;
            float peakSize = 200f;
            float duration = 0.25f; // time to grow/shrink
            float elapsed = 0f;

            // Grow
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                comboText.fontSize = Mathf.Lerp(startSize, peakSize, t);
                comboText.text = $"{comboStep + 1}X";
                elapsed += Time.deltaTime;
                yield return null;
            }
            comboText.fontSize = peakSize;

            // Shrink
            elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                comboText.fontSize = Mathf.Lerp(peakSize, startSize, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            comboText.fontSize = startSize;
        }
    }

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
