using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private CinemachineCamera freeLookCam;
        [SerializeField] private InputReader input;

        [Header("Settings")] [SerializeField] private float moveSpeed = 6.0f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private float animationSmooth = 8f;
        
        //Animator parameters 
        static readonly int MoveX = Animator.StringToHash("MoveX");
        static readonly int MoveZ = Animator.StringToHash("MoveZ");

        private Transform mainCam;
        private Animator animator;
        private Vector2 animMove;
        private float currentSpeed;
        private Vector3 adjustedDirection;
        private float velocity;
        private Vector3 movement;
        private Rigidbody rb;

        private void Awake()
        {
            mainCam = Camera.main.transform;
            animator = GetComponentInChildren<Animator>();
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;
            freeLookCam.OnTargetObjectWarped(transform,
                transform.position - freeLookCam.transform.position - Vector3.forward);
            rb =  GetComponent<Rigidbody>(); 
            rb.freezeRotation = true;
        }

        private void Update()
        {
            movement = new Vector3(input.Direction.x, 0, input.Direction.y).normalized;
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void UpdateAnimator()
        {
            var target = new Vector2(movement.x, movement.z);
            animMove = Vector2.Lerp(animMove, target, animationSmooth * Time.deltaTime);
            animator.SetFloat(MoveX, adjustedDirection.x);
            animator.SetFloat(MoveZ, adjustedDirection.z);
        }

        private void HandleMovement()
        {
            //Rotate movement direction to match camera rotation
            adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
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
