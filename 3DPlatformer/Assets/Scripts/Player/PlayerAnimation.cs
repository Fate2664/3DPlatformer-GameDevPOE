using System;
using UnityEngine;

namespace Platformer
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private InputReader inputReader;
        private PlayerState playerState;
        
        private static int inputXHash =  Animator.StringToHash("MoveX");
        private static int inputYHash =  Animator.StringToHash("MoveY");
        private static int inputMagnitudeHash =  Animator.StringToHash("MoveMagnitude");
        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isJumpingHash = Animator.StringToHash("isJumping");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        
        private Vector3 currentBlendInput =  Vector3.zero;

        private void Awake()
        {
            inputReader = GetComponent<InputReader>();
            playerState = GetComponent<PlayerState>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            bool isIdling = playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isJumping = playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            bool isFalling = playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isGrounded = playerState.IsGroundedState();
            
            Vector2 inputTarget = isSprinting ? inputReader.MovementInput * 1.5f : inputReader.MovementInput;
            currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
            
            animator.SetBool(isGroundedHash, isGrounded);
            animator.SetBool(isJumpingHash, isJumping);
            animator.SetBool(isFallingHash, isFalling);
            
            animator.SetFloat(inputXHash, currentBlendInput.x);
            animator.SetFloat(inputYHash, currentBlendInput.y);
            animator.SetFloat(inputMagnitudeHash, currentBlendInput.magnitude);
        }
    }
}
