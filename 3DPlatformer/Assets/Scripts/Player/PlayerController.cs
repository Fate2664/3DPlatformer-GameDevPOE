using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour, IRespawnable
    {
        #region Class Variables

        [Header("References")] [SerializeField]
        private Camera playerCamera;

        [Header("Movement Settings")] [SerializeField]
        private float moveSpeed = 6.0f;
        [SerializeField] private float groundLinearDamping = 6f;
        [SerializeField] private float walkAcceleration = 0.15f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float slopeLimit = 45f;
        [SerializeField] private float stepOffset = 0.03f;
        [Space(10)] [SerializeField] private float runSpeed = 12f;
        [SerializeField] private float runAcceleration = 0.25f;
        [Space(10)] [SerializeField] private float jumpForce = 1.0f;
        [SerializeField] private float inAirAcceleration = 0.15f;
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private float inAirLinearDamping = 0f;

        [Space(10)] [Header("Climbing Settings")] [SerializeField]
        private float climbSpeed = 15f;

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
        private Rigidbody rb;
        private CapsuleCollider col;

        private float currentSpeed;
        private float moveDeadZone = 0.1f;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerRotation = Vector2.zero;
        private float targetYaw;
        private float targetPitch;
        private float movingThreshold = 0.01f;
        private bool jumpQueued;
        private float privStepOffset;
        private float jumpCooldownTimer = 0f;
        private RaycastHit currentWallHit;
        private bool isClimbing;
        private float ledgeProbeHeight = 1.2f;
        private float ledgeProbeForward = 0.45f;
        private float ledgeProbeDown = 2.0f;
        private PlatformMover activePlatform;

        private PlayerMovementState lastMoveState = PlayerMovementState.Falling;

        #endregion

        #region Startup Methods

        private void Awake()
        {
            input = GetComponent<InputReader>();
            playerState = GetComponentInChildren<PlayerState>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();
            privStepOffset = stepOffset;
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
            if (input.JumpPressed)
                jumpQueued = true;
            targetYaw += lookSenseH * input.LookInput.x;
            targetPitch = Mathf.Clamp(targetPitch - lookSenseV * input.LookInput.y, -lookLimitV, lookLimitV);
        }

        private void FixedUpdate()
        {
            if (jumpCooldownTimer > 0f)
                jumpCooldownTimer -= Time.fixedDeltaTime;
            
            Quaternion bodyRotation = Quaternion.Euler(0f, targetYaw, 0f);
            rb.MoveRotation(bodyRotation);
            
            Vector3 moveDirection = GetCameraRelativeMoveDirection();
            UpdateClimbState(moveDirection);
            UpdateMovementState();
            
            bool isGrounded = IsGrounded();
            rb.linearDamping = isClimbing ? 0f : isGrounded ? groundLinearDamping : inAirLinearDamping;
            
           
            
            if (isClimbing)
                HandleClimbMovement();
            else
                HandleHorizontalMovement(moveDirection, isGrounded);

            HandleJump(isGrounded);
            jumpQueued = false;
        }

        private void LateUpdate()
        {
            playerCamera.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
        }

        #endregion

        #region UpdateState Methods

        private void UpdateMovementState()
        {
            lastMoveState = playerState.CurrentPlayerMovementState;

            if (isClimbing)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Climbing);
                stepOffset = 0f;
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

            if (!isGrounded && rb.linearVelocity.y > 0f)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
                stepOffset = 0f;
            }
            else if (!isGrounded && rb.linearVelocity.y <= 0f)
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Falling);
                stepOffset = 0f;
            }
            else
            {
                stepOffset = privStepOffset;
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
                stepOffset = 0f;
                return;
            }

            if (isClimbing && TryMoveOffWall())
            {
                isClimbing = false;
                stepOffset = privStepOffset;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                return;
            }

            isClimbing = false;
        }

        private void ResetToRespawnState()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            jumpCooldownTimer = 0f;
            isClimbing = false;
            lastMoveState = PlayerMovementState.Idling;
            stepOffset = privStepOffset;
            playerState.SetPlayerMovementState(PlayerMovementState.Idling);
        }

        #endregion

        #region Movement & Camera Methods

        private Vector3 GetCameraRelativeMoveDirection()
        {
            Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z)
                .normalized;
            Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z)
                .normalized;
            return cameraRightXZ * input.MovementInput.x + cameraForwardXZ * input.MovementInput.y;
        }

        void HandleHorizontalMovement(Vector3 moveDirection, bool isGrounded)
        {
            bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            //Check Acceleration
            float horizontalAcceleration = !isGrounded ? inAirAcceleration :
                isSprinting ? runAcceleration : walkAcceleration;

            //Check speed
            float speed = !isGrounded ? runSpeed :
                isSprinting ? runSpeed : moveSpeed;

            Vector3 platformVelocity = activePlatform != null ? activePlatform.Velocity : Vector3.zero;
            Vector3 velocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            Vector3 targetVelocity = moveDirection * speed;
            
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, horizontalAcceleration * Time.fixedDeltaTime);            
            velocity.x = horizontalVelocity.x + platformVelocity.x;
            velocity.z = horizontalVelocity.z + platformVelocity.z;
            
            if (!isGrounded)
                velocity = HandleSteepWalls(velocity);

            rb.linearVelocity = velocity;
        }

        private void HandleJump(bool isGrounded)
        {
            if (!jumpQueued || !isGrounded || jumpCooldownTimer > 0f)
                return;
            
            Vector3 platformVelocity = activePlatform != null ? activePlatform.Velocity : Vector3.zero;
            Vector3 velocity = rb.linearVelocity;
            velocity.x += platformVelocity.x;
            velocity.z += platformVelocity.z;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            jumpCooldownTimer = jumpCooldown;
        }

        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            if (!TryGetGroundHit(out RaycastHit hit))
                return velocity;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > slopeLimit && rb.linearVelocity.y <= 0f)
                velocity = Vector3.ProjectOnPlane(velocity, hit.normal);

            return velocity;
        }
        #endregion
        
        #region Climbing

        private bool TryGetClimbWall(Vector3 moveDir, out RaycastHit hit)
        {
            Vector3 probeDir = moveDir.sqrMagnitude > 0.001f ? moveDir.normalized : transform.forward;

            if (!CharacterControllerUtils.TryGetWallHit(col, probeDir, climbCheckDistance,
                    climbableLayer, out hit))
                return false;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > slopeLimit + 1f && angle <= maxClimbAngle;
        }

        private bool TryMoveOffWall()
        {
            Vector3 wallNormal = currentWallHit.normal;
            if (wallNormal == Vector3.zero)
                return false;

            Vector3 center = transform.TransformPoint(col.center);

            //Probe from above the player and slightly over the ledge
            Vector3 probeOrigin = center + Vector3.up * ledgeProbeHeight - wallNormal * ledgeProbeForward;

            if (!Physics.Raycast(probeOrigin, Vector3.down, out RaycastHit topHit, ledgeProbeDown,
                    groundLayer | climbableLayer, QueryTriggerInteraction.Ignore))
                return false;

            float topAngle = Vector3.Angle(topHit.normal, Vector3.up);
            if (topAngle > slopeLimit)
                return false;

            Vector3 targetCenter =
                topHit.point + Vector3.up * ledgeSnapUp - wallNormal * ledgeSnapForward;

            transform.position = targetCenter;

            return true;
        }

        private void HandleClimbMovement()
        {
            Vector3 wallNormal = currentWallHit.normal;
            Vector3 wallUp = Vector3.ProjectOnPlane(Vector3.up, wallNormal).normalized;
            Vector3 wallRight = Vector3.Cross(wallUp, wallNormal).normalized;

            Vector3 desired = wallRight * input.MovementInput.x + wallUp * input.MovementInput.y;
            desired = Vector3.ClampMagnitude(desired, 1f) * climbSpeed;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.linearVelocity = desired;
        }

        #endregion

        #region Respawning

        public void RespawnAt(RespawnPointData checkpoint)
        {
            ResetToRespawnState();

            Vector3 rotation = checkpoint.Rotation.eulerAngles;
            cameraRotation.x = rotation.y;
            cameraRotation.y = 0f;
            
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = checkpoint.Position;
            rb.rotation = checkpoint.Rotation;
            
            playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
        }

        #endregion

        #region State Checks

        private bool IsMovingHorizontally()
        {
            Vector3 horizontalVelocity =
                new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            return horizontalVelocity.magnitude > movingThreshold;
        }

        private bool IsGrounded()
        {
            return TryGetGroundHit(out _);
        }

        private bool TryGetGroundHit(out RaycastHit hit)
        {
            Vector3 center = transform.TransformPoint(col.center);
            float halfSegment = Mathf.Max(0f, (col.height * 0.5f) - col.radius);
            Vector3 bottomSphereCenter = center - Vector3.up * halfSegment;

            const float skin = 0.02f;
            float castDistance = groundDistance + skin;

            if (!Physics.SphereCast(
                    bottomSphereCenter + Vector3.up * skin,
                    col.radius * 0.95f,
                    Vector3.down,
                    out hit,
                    castDistance,
                    groundLayer | climbableLayer,
                    QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle <= slopeLimit;
        }

        #endregion

        public void SetMovingPlatform(PlatformMover platform)
        {
            activePlatform = platform;
        }

        private void OnDrawGizmosSelected()
        {
            CapsuleCollider cc = GetComponent<CapsuleCollider>();
            if (cc == null) return;
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - cc.radius + 0.1f,
                transform.position.z);
            bool grounded = Physics.CheckSphere(spherePosition, cc.radius, groundLayer, QueryTriggerInteraction.Ignore);

            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(spherePosition, cc.radius);
        }
    }
}