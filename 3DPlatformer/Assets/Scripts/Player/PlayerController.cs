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

        [SerializeField] private float gravity = 25f;
        [SerializeField] private float drag = 0.01f;
        [SerializeField] private float walkAcceleration = 0.15f;
        [SerializeField] private float rotationSpeed = 15f;
        [Space(10)] [SerializeField] private float runSpeed = 12f;
        [SerializeField] private float runAcceleration = 0.25f;
        [Space(10)] [SerializeField] private float jumpForce = 1.0f;
        [SerializeField] private float inAirAcceleration = 0.15f;
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private float inAirDrag = 0.001f;
        
        [Space(10)] [Header("Climbing Settings")] 
        [SerializeField] private float climbSpeed = 15f;
        [SerializeField] private float climbCheckDistance = 0.3f;
        [SerializeField] private float maxClimbAngle = 90f;
        [SerializeField] private float ledgeSnapForward = 0.35f;
        [SerializeField] private float ledgeSnapUp = 0.08f;

        [Space(10)] [Header("Camera Settings")] [SerializeField]
        private float lookSenseH = 0.1f;

        [SerializeField] private float lookSenseV = 0.1f;
        [SerializeField] private float lookLimitV = 70f;

        [Space(10)] [Header("Environmental Details")] [SerializeField]
        private LayerMask groundLayer;

        [SerializeField] private float groundDistance = 0.5f;
        [SerializeField] private LayerMask climbableLayer;


        private InputReader input;
        private PlayerState playerState;
        private Animator animator;
        private CharacterController characterController;

        private float currentSpeed;
        private float moveDeadZone = 0.1f;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerRotation = Vector2.zero;
        private float movingThreshold = 0.01f;
        private float verticalVelocity = 0f;
        private float antiBump;
        private float jumpCooldownTimer = 0f;
        private float stepOffset;
        private RaycastHit currentWallHit;
        private bool isClimbing;
        private float ledgeProbeHeight = 1.2f;
        private float ledgeProbeForward = 0.45f;
        private float ledgeProbeDown = 2.0f;

        private PlayerMovementState lastMoveState = PlayerMovementState.Falling;

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

            Vector3 moveDirection = GetCameraRelativeMoveDirection();

            UpdateClimbState(moveDirection);
            UpdateMovementState();
            HandleVerticalMovement();

            if (isClimbing)
                HandleClimbMovement();
            else
                HandleHorizontalMovement(moveDirection);
        }

        private void LateUpdate()
        {
            HandleRotation();
        }

        #endregion

        private void UpdateMovementState()
        {
            lastMoveState = playerState.CurrentPlayerMovementState;

            if (isClimbing)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Climbing);
                characterController.stepOffset = 0f;
                return;
            }

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

            if (!isGrounded && verticalVelocity > 0f)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
                characterController.stepOffset = 0f;
            }
            else if (!isGrounded && verticalVelocity <= 0f)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Falling);
                characterController.stepOffset = 0f;
            }
            else
            {
                characterController.stepOffset = stepOffset;
            }
        }

        private void UpdateClimbState(Vector3 moveDir)
        {
            bool isPressingIntoWall =
                moveDir.sqrMagnitude > 0.001f && Vector3.Dot(moveDir.normalized, transform.forward) > 0f;

            if (isPressingIntoWall && TryGetClimbWall(moveDir, out RaycastHit wallHit))
            {
                isClimbing = true;
                currentWallHit = wallHit;
                characterController.stepOffset = 0f;
                return;
            }

            if (isClimbing && TryMoveOffWall())
            {
                isClimbing = false;
                characterController.stepOffset = stepOffset;
                verticalVelocity = 0f;
                return;
            }

            isClimbing = false;
        }

        #region Movement & Camera Methods

        private Vector3 GetCameraRelativeMoveDirection()
        {
            Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z)
                .normalized;
            Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z)
                .normalized;
            return cameraRightXZ * input.MovementInput.x + cameraForwardXZ * input.MovementInput.y;
        }

        void HandleHorizontalMovement(Vector3 moveDirection)
        {
            bool isGrounded = IsGrounded();
            bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            //Check Acceleration
            float horizontalAcceleration = !isGrounded ? inAirAcceleration :
                isSprinting ? runAcceleration : walkAcceleration;

            //Check speed
            float speed = !isGrounded ? runSpeed :
                isSprinting ? runSpeed : moveSpeed;

            Vector3 moveDelta = moveDirection * horizontalAcceleration * Time.deltaTime;
            Vector3 velocity = characterController.velocity + moveDelta;
            //Add drag
            float dragMagnitude = isGrounded ? drag : inAirDrag;
            Vector3 currentDrag = velocity.normalized * dragMagnitude;
            velocity = (velocity.magnitude > dragMagnitude) ? velocity - currentDrag : Vector3.zero;

            velocity = Vector3.ClampMagnitude(new Vector3(velocity.x, 0f, velocity.z), speed);
            velocity.y += verticalVelocity;
            velocity = !IsGroundedWhileAirborne() ? HandleSteepWalls(velocity) : velocity;

            characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleVerticalMovement()
        {
            if (isClimbing)
            {
                verticalVelocity = 0f;
                return;
            }

            bool isGrounded = IsGrounded();

            verticalVelocity -= gravity * Time.deltaTime;

            if (isGrounded && verticalVelocity < 0f)
                verticalVelocity = -antiBump;

            if (input.JumpPressed && isGrounded && jumpCooldownTimer <= 0f)
            {
                verticalVelocity += Mathf.Sqrt(jumpForce * 3 * gravity);
                jumpCooldownTimer = jumpCooldown;
            }

            if (playerState.IsStateGroundedState(lastMoveState) && !isGrounded)
                verticalVelocity += antiBump;
        }

        private void HandleRotation()
        {
            cameraRotation.x += lookSenseH * input.LookInput.x;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * input.LookInput.y, -lookLimitV, lookLimitV);

            Quaternion targetRotation = Quaternion.Euler(0f, cameraRotation.x, 0f);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
        }


        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController, groundLayer | climbableLayer);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= characterController.slopeLimit + 0.5f;
            if (!validAngle && verticalVelocity <= 0f)
                velocity = Vector3.ProjectOnPlane(velocity, normal);

            return velocity;
        }

        private bool TryGetClimbWall(Vector3 moveDir, out RaycastHit hit)
        {
            Vector3 probeDir = moveDir.sqrMagnitude > 0.001f ? moveDir.normalized : transform.forward;

            if (!CharacterControllerUtils.TryGetWallHit(characterController, probeDir, climbCheckDistance,
                    climbableLayer, out hit))
                return false;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > characterController.slopeLimit + 1f && angle <= maxClimbAngle;
            print(TryGetClimbWall(moveDir, out hit));
        }

        private bool TryMoveOffWall()
        {
            Vector3 wallNormal = currentWallHit.normal;
            if (wallNormal == Vector3.zero)
                return false;
            
            Vector3 center = transform.TransformPoint(characterController.center);
            
            //Probe from above the player and slightly over the ledge
            Vector3 probeOrigin = center + Vector3.up * ledgeProbeHeight - wallNormal * ledgeProbeForward;

            if (!Physics.Raycast(probeOrigin, Vector3.down, out RaycastHit topHit, ledgeProbeDown,
                    groundLayer | climbableLayer, QueryTriggerInteraction.Ignore))
                return false;
            
            float topAngle = Vector3.Angle(topHit.normal, Vector3.up);
            if (topAngle > characterController.slopeLimit)
                return false;
            
            Vector3 targetCenter =
                topHit.point + Vector3.up * ledgeSnapUp - wallNormal * ledgeSnapForward;

            characterController.enabled = false;
            transform.position = targetCenter;
            characterController.enabled = true;
            
            return true;
        }

        private void HandleClimbMovement()
        {
            Vector3 wallNormal = currentWallHit.normal;
            Vector3 wallUp = Vector3.ProjectOnPlane(Vector3.up, wallNormal).normalized;
            Vector3 wallRight = Vector3.Cross(wallUp, wallNormal).normalized;

            Vector3 desired = wallRight * input.MovementInput.x + wallUp * input.MovementInput.y;
            desired = Vector3.ClampMagnitude(desired, 1f) * climbSpeed;

            verticalVelocity = 0f;

            characterController.Move(desired * Time.deltaTime);
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
                transform.position.y - characterController.radius + 0.1f, transform.position.z);
            bool grounded = Physics.CheckSphere(spherePosition, characterController.radius, groundLayer | climbableLayer,
                QueryTriggerInteraction.Ignore);
            return grounded;
        }

        private bool IsGroundedWhileAirborne()
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController, groundLayer | climbableLayer);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= characterController.slopeLimit + 0.5f;

            return characterController.isGrounded && validAngle;
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc == null) return;
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - cc.radius + 0.1f,
                transform.position.z);
            bool grounded = Physics.CheckSphere(spherePosition, cc.radius, groundLayer, QueryTriggerInteraction.Ignore);

            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(spherePosition, cc.radius);
        }
    }
}