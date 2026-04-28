using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour, IRespawnable
    {
        #region Serialized Class Variables

        [Header("References")] [SerializeField]
        private Camera playerCamera;

        [SerializeField] private Transform playerRoot;

        [Header("Movement Settings")] [SerializeField]
        private float walkSpeed = 6.0f;

        [SerializeField] private float groundLinearDamping = 6f;
        [SerializeField] private float walkAcceleration = 0.15f;
        [SerializeField] private float slopeLimit = 45f;
        [SerializeField] private float stepOffset = 0.03f;
        [SerializeField] private float stepUpSpeed = 2.5f; 
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

        //This allows us to see in editor but not edit it
        [field: SerializeField]
        public PlayerLocomotionState CurrentPlayerLocomotionState { get; private set; } = PlayerLocomotionState.Idling;

        #endregion

        #region Private Variables

        private InputReader input;
        private Rigidbody rb;
        private CapsuleCollider col;
        private Animator animator;
        private StateMachine stateMachine;

        private Vector3 moveDirection;
        private float targetYaw;
        private float targetPitch;
        private bool jumpQueued;
        private float privStepOffset;
        private float jumpCooldownTimer = 0f;
        private RaycastHit currentWallHit;
        //TODO move isClimbing logic to PlayerState logic
        private bool isClimbing;

        #endregion

        #region Startup Methods

        private void Awake()
        {
            input = GetComponent<InputReader>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();
            animator = GetComponentInChildren<Animator>();
            privStepOffset = stepOffset;
            rb.freezeRotation = true;
            animator.updateMode = rb.interpolation == RigidbodyInterpolation.None
                ? AnimatorUpdateMode.Fixed
                : AnimatorUpdateMode.Normal;

            //State Machine
            stateMachine = new StateMachine();

            //Declare States
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            var fallState = new FallingState(this, animator);
            var climbState = new ClimbingState(this, animator);
            
            //Define transitions
            Any(jumpState, new FuncPredicate(ShouldEnterJumpState));
            Any(fallState, new FuncPredicate(ShouldEnterFallState));
            Any(climbState, new FuncPredicate(ShouldEnterClimbState));
            At(jumpState, locomotionState, new FuncPredicate(ShouldEnterLocomotionState)); //This should never really happen -> jump should always be followed by falling
            At(fallState, locomotionState, new FuncPredicate(ShouldEnterLocomotionState));
            At(climbState, locomotionState, new FuncPredicate(ShouldEnterLocomotionState));
            At(climbState, fallState, new FuncPredicate(ShouldEnterFallState));

            //Set initial state 
            stateMachine.SetState(locomotionState);
        }

        private void At(IState from, IState to, IPredicate condition) =>
            stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            targetYaw = transform.eulerAngles.y;
            targetPitch = playerCamera.transform.eulerAngles.x;
        }

        #endregion

        #region Update Logic

        private void Update()
        {
            targetYaw += lookSenseH * input.LookInput.x;
            targetPitch = Mathf.Clamp(targetPitch - lookSenseV * input.LookInput.y, -lookLimitV, lookLimitV);

            moveDirection = GetCameraRelativeMoveDirection();
            rb.linearDamping = isClimbing ? 0f : IsGrounded() ? groundLinearDamping : inAirLinearDamping;

            if (input.JumpPressed)
                jumpQueued = true;

            stateMachine.Update();
            UpdateLocomotionState();
        }

        private void FixedUpdate()
        {
            if (jumpCooldownTimer > 0f)
                jumpCooldownTimer -= Time.fixedDeltaTime;

            CheckClimbingState(moveDirection);
            stateMachine.FixedUpdate();
            
            jumpQueued = false;
        }

        private void LateUpdate()
        {
            playerCamera.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            playerRoot.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        }

        #endregion

        #region Update State Methods
        
        public void SetPlayerLocomotionState(PlayerLocomotionState playerLocomotionState)
        {
            CurrentPlayerLocomotionState = playerLocomotionState;
        }

        private void UpdateLocomotionState()
        {
            if (isClimbing || stateMachine.IsInState<JumpState>() || stateMachine.IsInState<FallingState>())
            {
                stepOffset = 0f;
                return;
            }

            bool isMoving = input.MovementInput != Vector2.zero;
            bool isMovingHorizontally = IsMovingHorizontally();
            bool isSprinting = isMovingHorizontally && input.SprintToggledOn;

            SetPlayerLocomotionState(isSprinting
                ? PlayerLocomotionState.Sprinting
                : isMovingHorizontally || isMoving
                    ? PlayerLocomotionState.Walking
                    : PlayerLocomotionState.Idling);
            
            stepOffset = privStepOffset;
        }

        private void ResetToRespawnState()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            jumpCooldownTimer = 0f;
            isClimbing = false;
            stepOffset = privStepOffset;
            SetPlayerLocomotionState(PlayerLocomotionState.Idling);
        }

        #endregion

        #region Movement & Camera Methods

        private Vector3 GetCameraRelativeMoveDirection()
        {
            Quaternion yawRotation = Quaternion.Euler(0f, targetYaw, 0f);
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 right = yawRotation * Vector3.right;

            return Vector3.ClampMagnitude(right * input.MovementInput.x + forward * input.MovementInput.y, 1f);
        }

        private Vector3 GetLookForwardXZ()
        {
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            return forward.sqrMagnitude > 0.001f ? forward.normalized : Vector3.forward;
        }

        public void HandleMovement()
        {
            HandleHorizontalMovement(moveDirection, IsGrounded());
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection, bool isGrounded)
        {
            bool isSprinting = isGrounded && input.SprintToggledOn;

            //Check Acceleration
            float horizontalAcceleration = !isGrounded ? inAirAcceleration :
                isSprinting ? runAcceleration : walkAcceleration;

            //Check speed
            float moveSpeed = !isGrounded ? runSpeed : isSprinting ? runSpeed : walkSpeed;

            Vector3 velocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            Vector3 targetVelocity = Vector3.ClampMagnitude(adjustedDirection, 1f) * moveSpeed;

            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity,
                horizontalAcceleration * Time.fixedDeltaTime);
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;

            //Step check
            if (isGrounded &&
                CharacterControllerUtils.TryGetStepOffset(col,
                    new Vector3(velocity.x, 0f, velocity.z) * Time.fixedDeltaTime, stepOffset, slopeLimit,
                    groundLayer | climbableLayer, groundLayer, out Vector3 stepDelta))
            {
                float stepUp = Mathf.Min(stepDelta.y, stepUpSpeed * Time.fixedDeltaTime);

                if (stepUp > 0f)
                    rb.position += Vector3.up * stepUp;
                
                velocity.y = Mathf.Max(velocity.y, 0f);
            }

            //Steep walls check
            if (!isGrounded)
                velocity = HandleSteepWalls(velocity);

            //Final velocity
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        public void HandleJump()
        {
            if (!jumpQueued || !IsGrounded() || jumpCooldownTimer > 0f)
                return;

            Vector3 velocity = rb.linearVelocity;
            rb.linearVelocity = velocity;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpCooldownTimer = jumpCooldown;
        }

        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            if (!CharacterControllerUtils.TryGetGroundHit(out RaycastHit hit, transform, col, groundLayer,
                    climbableLayer, slopeLimit, groundDistance))
                return velocity;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > slopeLimit && rb.linearVelocity.y <= 0f)
                velocity = Vector3.ProjectOnPlane(velocity, hit.normal);

            return velocity;
        }

        #endregion

        #region Climbing
        
        private void CheckClimbingState(Vector3 moveDir)
        {
            bool isPressingIntoWall =
                moveDir.sqrMagnitude > 0.001f && Vector3.Dot(moveDir.normalized, GetLookForwardXZ()) > 0f;

            if (isPressingIntoWall && CharacterControllerUtils.TryGetClimbWall(col, moveDir, out RaycastHit wallHit,
                    GetLookForwardXZ(), climbCheckDistance, climbableLayer, slopeLimit, maxClimbAngle))
            {
                isClimbing = true;
                currentWallHit = wallHit;
                stepOffset = 0f;
                return;
            }

            if (isClimbing && CharacterControllerUtils.TryMoveOffWall(currentWallHit, transform, col, groundLayer,
                    climbableLayer, slopeLimit, ledgeSnapUp, ledgeSnapForward))
            {
                isClimbing = false;
                stepOffset = privStepOffset;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                return;
            }

            isClimbing = false;
        }

        public void HandleClimbMovement()
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

            targetYaw = checkpoint.Rotation.eulerAngles.y;
            targetPitch = 0f;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = checkpoint.Position;
            rb.rotation = checkpoint.Rotation;

            playerCamera.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            playerRoot.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        }

        #endregion

        #region State Checks

        private bool IsMovingHorizontally()
        {
            Vector3 horizontalVelocity =
                new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            return horizontalVelocity.magnitude > 0.01f;
        }

        private bool IsGrounded()
        {
            return CharacterControllerUtils.TryGetGroundHit(out _, transform, col, groundLayer, climbableLayer,
                slopeLimit, groundDistance);
        }

        private bool IsOnMovingPlatform()
        {
            return transform.parent != null && transform.parent.CompareTag("MovingPlatform");
        }

        private bool ShouldEnterJumpState()
        {
            return !isClimbing && !IsGrounded() && rb.linearVelocity.y > 0f;
        }

        private bool ShouldEnterFallState()
        {
            return !isClimbing && !IsGrounded() && rb.linearVelocity.y <= 0f;
        }

        private bool ShouldEnterLocomotionState()
        {
            return !isClimbing && IsGrounded();
        }

        private bool ShouldEnterClimbState()
        {
            return isClimbing;
        }

        #endregion
    }
}