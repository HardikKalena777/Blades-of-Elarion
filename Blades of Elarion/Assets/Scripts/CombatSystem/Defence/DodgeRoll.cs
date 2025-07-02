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

    // Store the roll direction
    private Vector3 rollDirection = Vector3.zero;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movementController == null)
            movementController = GetComponent<ThirdPersonController>();
    }

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

        // Get the current movement input direction
        Vector2 moveInput = movementController.GetComponent<StarterAssetsInputs>().move;

        // If no input, roll forward
        if (moveInput.sqrMagnitude < 0.01f)
            moveInput = Vector2.up;

        // Convert input to world direction
        rollDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        rollDirection.Normalize();

        // Optionally, set animator parameters for roll direction (if using a blend tree)
        animator.SetFloat("RollHorizontal", moveInput.x);
        animator.SetFloat("RollVertical", moveInput.y);

        animator.SetTrigger("Roll");
    }

    // Called via animation event at end of roll animation
    public void EndRoll()
    {
        isRolling = false;
        animator.applyRootMotion = false;
    }

    // Optionally, expose roll direction for use in root motion or movement scripts
    public Vector3 GetRollDirection()
    {
        return rollDirection;
    }
}
