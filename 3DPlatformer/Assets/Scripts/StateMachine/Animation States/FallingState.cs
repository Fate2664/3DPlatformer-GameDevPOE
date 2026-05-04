using UnityEngine;

namespace Platformer
{
    public class FallingState : PlayerBaseState
    {
        public FallingState(PlayerController player, Animator animator) : base(player, animator) { }
        public StopwatchTimer fallTimer = new ();
        
        public override void OnEnter()
        {
            animator.CrossFade(FallingHash, crossFadeDuration);
            fallTimer.Start();
        }

        public override void FixedUpdate()
        {
            player.HandleJump();
            player.HandleMovement();
            fallTimer.Tick(Time.fixedDeltaTime);            
        }

        public override void OnExit()
        {
            fallTimer.Stop();
        }
    }
}
