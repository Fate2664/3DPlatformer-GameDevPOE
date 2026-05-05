using System;
using UnityEngine;

namespace Platformer
{
    //This is the player animation script. Currently it manages the player locomotion animations as the player states manage the other animations
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private InputReader inputReader;
        private PlayerController playerController;
            
        //Get input hashes 
        private static int inputXHash =  Animator.StringToHash("MoveX");
        private static int inputYHash =  Animator.StringToHash("MoveY");
        private static int inputMagnitudeHash =  Animator.StringToHash("MoveMagnitude");
        
        private Vector3 currentBlendInput =  Vector3.zero;

        private void Awake()
        {
            inputReader = GetComponent<InputReader>();
            playerController = GetComponent<PlayerController>();
        }

        private void LateUpdate()
        {
            UpdateAnimationState();
        }
        
        //Update the animation state depending on the input from the player
        private void UpdateAnimationState()
        {
            bool isSprinting = playerController.CurrentPlayerLocomotionState == PlayerLocomotionState.Sprinting;
            
            Vector2 inputTarget = isSprinting ? inputReader.MovementInput * 1.5f : inputReader.MovementInput;
            currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
          
            animator.SetFloat(inputXHash, currentBlendInput.x);
            animator.SetFloat(inputYHash, currentBlendInput.y);
            animator.SetFloat(inputMagnitudeHash, currentBlendInput.magnitude);
        }
    }
}
