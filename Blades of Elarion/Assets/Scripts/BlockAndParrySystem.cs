using StarterAssets;
using System.Collections;
using UnityEngine;

public class BlockAndParrySystem : MonoBehaviour
{
    [Header("Settings")]
    public float parryWindow = 0.3f;
    public float blockStaminaDrain = 10f;

    [Header("References")]
    public Animator animator;
    public StaminaSystem staminaSystem;
    ThirdPersonController movementController;

    [Header("State")]
    public bool isBlocking;
    public bool isParrying;
    private float blockInputTime;
    private float tapThreshold = 0.25f; // Max time to consider as "tap"

    // These two properties let other scripts safely check player's state.
    public bool IsBlocking => isBlocking;
    public bool IsParrying => isParrying;

    private void Awake()
    {
        movementController = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.JoystickButton4)) // Mouse1 or Left Shoulder
        {
            blockInputTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.JoystickButton4))
        {
            float heldTime = Time.time - blockInputTime;

            if (heldTime <= tapThreshold)
            {
                // Tap → Parry
                TryParry();
            }
            else
            {
                // Was held long enough, stop blocking
                StopBlocking();
            }
        }

        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.JoystickButton4))
        {
            float heldTime = Time.time - blockInputTime;

            if (heldTime > tapThreshold && !isBlocking)
            {
                StartBlocking();
            }
        }
    }

    void TryParry()
    {
        if (isParrying || isBlocking) return;

        isParrying = true;
        animator.SetTrigger("Parry");
        StartCoroutine(ParryWindowCoroutine());
    }

    IEnumerator ParryWindowCoroutine()
    {
        float timer = 0f;

        // Enable parry detection
        // You can activate a parry collider here if needed
        while (timer < parryWindow)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isParrying = false;
    }

    void StartBlocking()
    {
        if (isParrying) return;

        isBlocking = true;
        animator.SetBool("IsBlocking", true);
        movementController.DisableMovement();
    }

    void StopBlocking()
    {
        isBlocking = false;
        animator.SetBool("IsBlocking", false);
        movementController.EnableMovement();
    }

    public void OnBlockedHit()
    {
        if (isBlocking)
        {
            staminaSystem.UseStamina(blockStaminaDrain);
            CameraShake.Instance.ShakeCamera(1f, 0.2f);
            HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f);
            // Add block reaction VFX, sound, etc.
        }
    }

    public bool IsParryActive()
    {
        return isParrying;
    }

    public bool IsBlocked()
    {
        return isBlocking;
    }
}
