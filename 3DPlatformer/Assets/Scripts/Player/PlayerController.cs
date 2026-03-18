using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Camera playerCamera;

        [Header("Settings")] [SerializeField] private float moveSpeed = 6.0f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float lookSenseH = 0.1f;
        [SerializeField] private float lookSenseV = 0.1f;
        [SerializeField] private float lookLimitV = 70f;
        [SerializeField] private float moveDeadZone = 0.1f;

        private InputReader input;
        private Animator animator;
        private CharacterController characterController;

        private float currentSpeed;
        private float velocity;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerRotation = Vector2.zero;

        private void Awake()
        {
            input = GetComponent<InputReader>();
            animator = GetComponentInChildren<Animator>();
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleMovement();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            HandleRotation();
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
            Vector3 inputMove = Vector3.ClampMagnitude(moveDirection, 1f);
            if (inputMove.sqrMagnitude < moveDeadZone * moveDeadZone)
            {
                inputMove = Vector3.zero;
            }

            Vector3 horizontalMove = inputMove * moveSpeed;
            characterController.Move(horizontalMove * Time.deltaTime);
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
    }
}

