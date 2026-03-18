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
            Vector2 inputTarget = inputReader.MovementInput;
            currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
            
            animator.SetFloat(inputXHash, currentBlendInput.x);
            animator.SetFloat(inputYHash, currentBlendInput.y);
        }
    }
}
