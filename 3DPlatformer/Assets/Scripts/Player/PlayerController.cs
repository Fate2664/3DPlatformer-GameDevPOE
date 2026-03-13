using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private CharacterController controller;

        [SerializeField] private Animator animator;
        [SerializeField] private CinemachineCamera freeLookCam;
        [SerializeField] private InputReader input;

        [Header("Settings")] [SerializeField] private float moveSpeed = 6.0f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float smoothTime = 0.2f;
        
        //Animator parameters 
        static readonly int Speed = Animator.StringToHash("Speed");

        private Transform mainCam;
        private float currentSpeed;
        private float velocity;


        private void Awake()
        {
            mainCam = Camera.main.transform;
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;
            freeLookCam.OnTargetObjectWarped(transform,
                transform.position - freeLookCam.transform.position - Vector3.forward);
        }

        private void Update()
        {
            HandleMovement();
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }

        private void HandleMovement()
        {
            var moveDirection = new Vector3(input.Direction.x, 0, input.Direction.y).normalized;
            //Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * moveDirection;
            if (adjustedDirection.magnitude > 0f)
            {
                HandleRotation(adjustedDirection);
                HandleCharacterController(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(0f);
            }
        }

        void HandleCharacterController(Vector3 adjustedDirection)
        {
            //Move player
            var adjustedMovement = adjustedDirection * (moveSpeed * Time.deltaTime);
            controller.Move(adjustedMovement);
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
