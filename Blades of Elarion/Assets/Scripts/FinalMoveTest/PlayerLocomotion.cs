using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Scripts")]
    InputManager inputManager;
    PlayerManager playerManager;
    AnimationManager animationManager;
    Features features;

    [Header("Componenets")]
    Vector3 moveDirection;
    Transform cameraObj;
    Rigidbody playerRigidBody;

    [Header("Falling"), Space(5)]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isJogging;
    [HideInInspector] public bool isWalking;

    [Header("Movement Speeds"),Space(5)]
    public float walkSpeed = 1.5f;
    public float jogSpeed = 5f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;
    private (float walkSpeed, float jogSpeed, float runSpeed, float rotationSpeed) storedSpeeds;

    [Header("Jump Speeds")]
    public float jumpHeight = 3f;
    public float gravityIntensity = -15f;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        animationManager = GetComponent<AnimationManager>();
        features = GetComponent<Features>();
        playerRigidBody = GetComponent<Rigidbody>();
        cameraObj = Camera.main.transform;
    }

    private void Start()
    {
        StoreSpeeds();
        ToggleCursor(false);
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if(isJumping) 
            return;
        HandleFallingAndLanding();

        if(playerManager.isInteracting) 
            return;

        moveDirection = cameraObj.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObj.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(isRunning && features.jogging)
        {
            moveDirection = moveDirection * runSpeed;
        }
        else
        {
            if(inputManager.moveAmount > 0.5f && features.jogging)
            {
                moveDirection = moveDirection * jogSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkSpeed;
            }
        }

        Vector3 movementVelocity = moveDirection;
        playerRigidBody.linearVelocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if(isJumping) return;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObj.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObj.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position + Vector3.up * rayCastHeightOffset;
        Vector3 targetPosition = transform.position;

        bool hitGround = Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, rayCastHeightOffset + 0.1f, groundLayer);

        if (hitGround)
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animationManager.PlayTargetAnimation("Land", true, 0.1f);
            }

            targetPosition.y = hit.point.y;
            inAirTimer = 0f;
            isGrounded = true;
        }
        else
        {
            if (!isJumping && !playerManager.isInteracting)
            {
                animationManager.PlayTargetAnimation("Fall", true, 0.2f);
            }

            inAirTimer += Time.deltaTime;
            isGrounded = false;
        }

        if (isGrounded && !isJumping)
        {
            transform.position = inputManager.moveAmount > 0
                ? Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime / 0.01f)
                : targetPosition;
        }
    }

    public void HandleJumping()
    {
        if(isGrounded)
        {
            isGrounded = false;
            animationManager.animator.SetBool("IsJumping",true);
            animationManager.PlayTargetAnimation("Jump",false,0.1f);

            float jumpVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpVelocity;
            playerRigidBody.linearVelocity = playerVelocity;
        }
    }

    public void ToggleMovement(bool enabled)
    {
        if (enabled)
        {
            (walkSpeed, jogSpeed, runSpeed, rotationSpeed) = GetSpeeds();
        }
        else
        {
            walkSpeed = 0f;
            jogSpeed = 0f;
            runSpeed = 0f;
            rotationSpeed = 0f;
        }
    }

    public void ToggleCursor(bool enabled)
    {
        if(enabled)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void StoreSpeeds()
    {
        storedSpeeds = (walkSpeed, jogSpeed, runSpeed, rotationSpeed);
    }

    public (float walkSpeed, float jogSpeed, float runSpeed, float rotationSpeed) GetSpeeds()
    {
        return storedSpeeds;
    }
}
