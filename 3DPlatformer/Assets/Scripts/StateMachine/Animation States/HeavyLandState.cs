using UnityEngine;

namespace Platformer
{
    public class HeavyLandState : PlayerBaseState
    {
        public HeavyLandState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("Entering Heavy Land");
            animator.CrossFade(HeavyLandHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleJump();
        }
        
        
    }
}