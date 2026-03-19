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

        [Header("Movement Settings")] 
        [SerializeField] private float moveSpeed = 6.0f;
        [SerializeField] private float drag = 0.01f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float runSpeed = 12f;
        [SerializeField] private float moveDeadZone = 0.1f;
        
        [Space(10)]
        [Header("Camera Settings")] 
        [SerializeField] private float lookSenseH = 0.1f;
        [SerializeField] private float lookSenseV = 0.1f;
        [SerializeField] private float lookLimitV = 70f;
        
        private InputReader input;
        private PlayerState playerState;
        private Animator animator;
        private CharacterController characterController;

        private float currentSpeed;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerRotation = Vector2.zero;
        private float movingThreshold = 0.01f;

        #endregion

        #region Startup Methods

        private void Awake()
        {
            input = GetComponent<InputReader>();
            animator = GetComponentInChildren<Animator>();
            playerState = GetComponentInChildren<PlayerState>();
            characterController = GetComponent<CharacterController>();
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
            UpdateMovementState();
            HandleMovement();
        }

        private void LateUpdate()
        {
            HandleRotation();
        }

        #endregion

        private void UpdateMovementState()
        {
            bool isMoving = input.MovementInput !=  Vector2.zero;
            bool isMovingHorizontally = IsMovingHorizontally();
            bool isSprinting = isMovingHorizontally && input.SprintToggledOn;
            
            PlayerMovementState horizontalState = isSprinting ? PlayerMovementState.Sprinting :
                isMovingHorizontally || isMoving
                ? PlayerMovementState.Walking
                : PlayerMovementState.Idling;
            playerState.SetPlayerMovementState(horizontalState);
        }

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
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
            
            //Check speed
            float speed = isSprinting ? runSpeed : moveSpeed;
            Vector3 velocity = characterController.velocity + moveDirection;
            
            //Add drag
            Vector3 currentDrag = velocity.normalized * drag;
            velocity = (velocity.magnitude > drag) ? velocity - currentDrag : Vector3.zero;
            velocity = Vector3.ClampMagnitude(velocity, speed);
            
            characterController.Move(velocity * Time.deltaTime);
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
        
        private bool IsMovingHorizontally()
        {
            Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
            return horizontalVelocity.magnitude > movingThreshold;
        }
    }
}

