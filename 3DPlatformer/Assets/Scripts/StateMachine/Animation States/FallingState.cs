using UnityEngine;

namespace Platformer
{
    public class FallingState : BaseState
    {
        public FallingState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(FallingHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleJump();
            player.HandleMovement();
        }
    }
}
