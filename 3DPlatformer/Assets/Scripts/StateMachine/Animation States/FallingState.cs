using UnityEngine;

namespace Platformer
{
    public class FallingState : PlayerBaseState
    {
        //The falling state has a timer to check how long the player has been falling for. This will help when checking for heavy landings
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
