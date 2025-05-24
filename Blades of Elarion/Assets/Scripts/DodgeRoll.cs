using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DodgeRoll : MonoBehaviour
{
    [Header("Roll Settings")]
    public float staminaCost = 20f;
    public float rollCooldown = 0.8f;

    [Header("References")]
    public Animator animator;
    public StaminaSystem staminaSystem;
    public ThirdPersonController movementController;

    public bool isRolling = false;
    private float lastRollTime;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movementController == null)
            movementController = GetComponent<ThirdPersonController>();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton2))
    //    {
    //        HandleRollInput();
    //    }
    //}

    public void HandleRollInput()
    {
        if (Time.time >= lastRollTime + rollCooldown && !isRolling)
        {
            if (!staminaSystem.HasStamina(staminaCost))
                return;

            staminaSystem.UseStamina(staminaCost);
            StartRoll();
        }
    }

    void StartRoll()
    {
        isRolling = true;
        lastRollTime = Time.time;

        animator.SetTrigger("Roll");
    }

    // Called via animation event at end of roll animation
    public void EndRoll()
    {
        isRolling = false;
        animator.applyRootMotion = false;
    }
}
