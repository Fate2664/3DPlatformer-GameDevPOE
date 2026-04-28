using UnityEngine;

namespace Platformer
{
    public enum PlayerLocomotionState
    {
        Idling = 0,
        Walking = 1,
        Sprinting = 2,
    }
    
    public abstract class PlayerBaseState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");
        protected static readonly int FallingHash = Animator.StringToHash("Fall");
        protected static readonly int ClimbingHash = Animator.StringToHash("Climbing");
        
        protected const float crossFadeDuration = 0.3f;

        protected PlayerBaseState(PlayerController player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }
        
        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }
}
