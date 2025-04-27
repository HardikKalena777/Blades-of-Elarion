using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    AnimationManager animationManager;
    WeaponManager weaponManager;
    CombatManager combatManager;
    Features features;

    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public float moveAmount;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public bool runInput;
    [HideInInspector] public bool jumpInput;

    [HideInInspector] public bool drawInput;
    [HideInInspector] public bool sheathInput;

    [HideInInspector] public bool lightAttackInput;
    [HideInInspector] public bool heavyAttackInput;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        combatManager = GetComponent<CombatManager>();
        features = GetComponent<Features>();    
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            #region BasicMovementInput
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Run.performed += i => runInput = true;
            playerControls.PlayerActions.Run.canceled += i => runInput = false;

            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            #endregion

            playerControls.PlayerCombat.DrawWeapon.performed += i => drawInput = true;
            playerControls.PlayerCombat.SheathWeapon.performed += i => sheathInput = true;
            playerControls.PlayerCombat.LightAttack.performed += i => lightAttackInput = true;
            playerControls.PlayerCombat.HeavyAttack.performed += i => heavyAttackInput = true;
        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleWalkInput();
        HandleJogInput();
        HandleRunInput();
        HandleJumpInput();
        HandleAttackInput();
        HandleWeaponInput();
    }

    #region BasicMovement
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        animationManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isRunning, playerLocomotion.isJogging, playerLocomotion.isWalking);
    }

    private void HandleWalkInput()
    {
        if (!features.jogging && moveAmount > 0)
        {
            playerLocomotion.isWalking = true;
        }
        else
        {
            playerLocomotion.isWalking = false;
        }
    }

    private void HandleJogInput()
    {
        if (features.jogging && moveAmount > 0.5f && !playerLocomotion.isRunning)
        {
            playerLocomotion.isJogging = true;
        }
        else
        {
            playerLocomotion.isJogging = false;
        }
    }

    private void HandleRunInput()
    {
        if (runInput && moveAmount > 0.5f)
        {
            playerLocomotion.isRunning = true;
        }
        else
        {
            playerLocomotion.isRunning = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerLocomotion.HandleJumping();
        }
    }
    #endregion

    private void HandleWeaponInput()
    {
        if (drawInput)
        {
            drawInput = false;
            weaponManager.HandleWeaponToggle();
        }
    }

    private void HandleAttackInput()
    {
        if (lightAttackInput)
        {
            combatManager.LightAttack();
            lightAttackInput = false;
        }
        if (heavyAttackInput)
        {
            combatManager.HeavyAttack();
            heavyAttackInput = false;
        }
    }
}
