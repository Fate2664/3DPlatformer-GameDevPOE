using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables

        [Header("References")] [SerializeField]
        private Camera playerCamera;

        [Header("Movement Settings")] [SerializeField]
        private float moveSpeed = 6.0f;

        [SerializeField] private float drag = 0.01f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float runSpeed = 12f;
        [SerializeField] private float moveDeadZone = 0.1f;
        [SerializeField] private float gravity = 25f;
        [SerializeField] private float jumpForce = 1.0f;
        [SerializeField] private float jumpCooldown = 0.5f;

        [Space(10)] [Header("Camera Settings")] [SerializeField]
        private float lookSenseH = 0.1f;

        [SerializeField] private float lookSenseV = 0.1f;
        [SerializeField] private float lookLimitV = 70f;

        [Space(10)] [Header("Environmental Details")] [SerializeField]
        private LayerMask groundLayer;


        private InputReader input;
        private PlayerState playerState;
        private Animator animator;
        private CharacterController characterController;

        private float currentSpeed;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerRotation = Vector2.zero;
        private float movingThreshold = 0.01f;
        private float verticalVelocity = 0f;
        private float antiBump;
        private float jumpCooldownTimer = 0f;
        private float stepOffset;

        #endregion

        #region Startup Methods

        private void Awake()
        {
            input = GetComponent<InputReader>();
            animator = GetComponentInChildren<Animator>();
            playerState = GetComponentInChildren<PlayerState>();
            characterController = GetComponent<CharacterController>();
            antiBump = runSpeed;
            stepOffset = characterController.stepOffset;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        #endregion

        #region Update Logic

        private void Update()
        {
            if (jumpCooldownTimer > 0f)
                jumpCooldownTimer -= Time.deltaTime;
            UpdateMovementState();
            HandleVerticalMovement();
            HandleMovement();
        }

        private void LateUpdate()
        {
            HandleRotation();
        }

        #endregion

        private void UpdateMovementState()
        {
            bool isMoving = input.MovementInput != Vector2.zero;
            bool isMovingHorizontally = IsMovingHorizontally();
            bool isSprinting = isMovingHorizontally && input.SprintToggledOn;
            bool isGrounded = IsGrounded();

            PlayerMovementState horizontalState = isSprinting
                ? PlayerMovementState.Sprinting
                : isMovingHorizontally || isMoving
                    ? PlayerMovementState.Walking
                    : PlayerMovementState.Idling;
            playerState.SetPlayerMovementState(horizontalState);

            if (!isGrounded && characterController.velocity.y > 0f)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
                characterController.stepOffset = 0f;
            }
            else if (!isGrounded && characterController.velocity.y <= 0f)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Falling);
                characterController.stepOffset = 0f;
            }
            else
            {
                characterController.stepOffset = stepOffset;
            }
        }

        #region Movement & Camera Methods

        private void HandleMovement()
        {
            Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z)
                .normalized;
            Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z)
                .normalized;
            Vector3 moveDirection = cameraRightXZ * input.MovementInput.x + cameraForwardXZ * input.MovementInput.y;
            HandleHorizontalMovement(moveDirection);
        }

        void HandleHorizontalMovement(Vector3 moveDirection)
        {
            bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isGrounded = playerState.IsGroundedState();
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            //Check speed
            float speed = !isGrounded ? runSpeed :
                isSprinting ? runSpeed : moveSpeed;

            Vector3 velocity = characterController.velocity + moveDirection;

            //Add drag
            Vector3 currentDrag = velocity.normalized * drag;
            velocity = (velocity.magnitude > drag) ? velocity - currentDrag : Vector3.zero;
            velocity = Vector3.ClampMagnitude(new Vector3(velocity.x, 0f, velocity.z), speed);
            velocity.y += verticalVelocity;
            velocity = !isGrounded ? HandleSteepWalls(velocity) : velocity;

            characterController.Move(velocity * Time.deltaTime);
        }

        //Jumping
        private void HandleVerticalMovement()
        {
            bool isGrounded = playerState.IsGroundedState();

            verticalVelocity -= gravity * Time.deltaTime;

            if (isGrounded && verticalVelocity < 0f)
                verticalVelocity = -antiBump;


            if (input.JumpPressed && isGrounded && jumpCooldownTimer <= 0f)
            {
                verticalVelocity += antiBump + Mathf.Sqrt(jumpForce * 3f * gravity);
                jumpCooldownTimer = jumpCooldown;
            }
        }

        private void HandleRotation()
        {
            cameraRotation.x += lookSenseH * input.LookInput.x;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * input.LookInput.y, -lookLimitV, lookLimitV);

            var targetRotation = Quaternion.Euler(0f, cameraRotation.x, 0f);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
        }

        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController, groundLayer);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle < characterController.slopeLimit;

            if (!validAngle && verticalVelocity < 0f)
                velocity = Vector3.ProjectOnPlane(velocity, normal);

            return velocity;
        }

        #endregion

        #region State Checks

        private bool IsMovingHorizontally()
        {
            Vector3 horizontalVelocity =
                new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
            return horizontalVelocity.magnitude > movingThreshold;
        }

        private bool IsGrounded()
        {
            bool grounded = playerState.IsGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();

            return grounded;
        }

        private bool IsGroundedWhileGrounded()
        {
            Vector3 spherePosition = new Vector3(transform.position.x,
                transform.position.y - characterController.radius, transform.position.z);

            bool grounded = Physics.CheckSphere(spherePosition, characterController.radius, groundLayer,
                QueryTriggerInteraction.Ignore);
            return grounded;
        }

        private bool IsGroundedWhileAirborne()
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController, groundLayer);
            float angle = Vector3.Angle(normal, Vector3.up);
            print(angle);
            bool validAngle = angle < characterController.slopeLimit;

            return characterController.isGrounded && validAngle;
        }

        #endregion
    }
}