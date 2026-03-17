using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Camera playerCamera;
        [SerializeField] private InputReader input;

        [Header("Settings")] [SerializeField] private float moveSpeed = 6.0f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private float animationSmooth = 8f;
        [SerializeField] private float lookSenseH = 0.1f;
        [SerializeField] private float lookSenseV = 0.1f;
        
        //Animator parameters 
        static readonly int MoveX = Animator.StringToHash("MoveX");
        static readonly int MoveZ = Animator.StringToHash("MoveZ");

        private float lookLimitV = 70f;
        private Animator animator;
        private float currentSpeed;
        private Vector3 adjustedDirection;
        private float velocity;
        private Vector3 movement;
        private Rigidbody rb;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerRotation = Vector2.zero;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            rb =  GetComponent<Rigidbody>(); 
            rb.freezeRotation = true;
        }

        private void Update()
        {
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z).normalized;
            Vector3 moveDirection = cameraRightXZ * input.MovementInput.x + cameraForwardXZ * input.MovementInput.y;
            
            Vector3 velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.linearVelocity = velocity;   
            //HandleMovement();
        }

        private void LateUpdate()
        {
            cameraRotation.x += lookSenseH * input.LookInput.x;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * input.LookInput.y, -lookLimitV, lookLimitV);
            
            playerRotation.x = transform.eulerAngles.x + lookSenseH * input.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, playerRotation.x, 0f);
            
            playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
        }

        private void UpdateAnimator()
        {
            animator.SetFloat(MoveX, input.MovementInput.x);
            animator.SetFloat(MoveZ, input.MovementInput.y);
        }

        private void HandleMovement()
        {
            //Rotate movement direction to match camera rotation
            adjustedDirection = Quaternion.AngleAxis(cameraRotation.y, Vector3.up) * movement;
            if (adjustedDirection.magnitude > 0f)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(0f);
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            //Move player
            Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}
