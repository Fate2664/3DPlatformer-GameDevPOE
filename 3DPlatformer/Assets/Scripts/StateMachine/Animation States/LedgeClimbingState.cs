using UnityEngine;

namespace Platformer
{
    public class LedgeClimbingState : PlayerBaseState
    {
        public LedgeClimbingState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(ClimbOverLedgeHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleClimbMovement();
        }
    }
}