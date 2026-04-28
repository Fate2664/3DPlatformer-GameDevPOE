using UnityEngine;

namespace Platformer
{
    public class ClimbingState : PlayerBaseState
    {
        public ClimbingState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(ClimbingHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleClimbMovement();
        }
    }
}