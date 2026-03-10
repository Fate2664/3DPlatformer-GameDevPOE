using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private CharacterController controller;
        [SerializeField] private Animator animator;
        [SerializeField] private CinemachineFreeLook freeLookCam;
        [SerializeField] private InputReader input;
        
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 6.0f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float smoothTime = 0.2f;

        private Transform mainCam;

        private void Awake()
        {
            mainCam = Camera.main.transform;
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;
            freeLookCam.OnTargetObjectWarped(transform, transform.position - freeLookCam.transform.position -  Vector3.forward);
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            var moveDirection = new Vector3(input.Direction.x , 0, input.Direction.z).normalized;
            //Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * moveDirection;
            if (adjustedDirection.magnitude > 0f)
            {
                var targetRotation = Quaternion.LookRotation(adjustedDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.LookAt(transform.position + adjustedDirection);
                
                //Move player
                
            }
        }
    }
}
