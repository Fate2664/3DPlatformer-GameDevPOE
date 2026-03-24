using UnityEngine;

namespace Platformer
{
    public enum PlayerMovementState
    {
        Idling = 0,
        Walking = 1,
        Sprinting = 2,
        Jumping = 3,
        Falling = 4,
        Strafing = 5
    }
    
    public class PlayerState : MonoBehaviour
    {
        //This allows us to see in editor but not edit it
        [field: SerializeField]
        public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;
 
        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
        }

        public bool IsGroundedState()
        {
            return IsStateGroundedState(CurrentPlayerMovementState);
        }

        public bool IsStateGroundedState(PlayerMovementState movementState)
        {
            return movementState == PlayerMovementState.Idling ||
                   movementState == PlayerMovementState.Walking ||
                   movementState == PlayerMovementState.Sprinting;
        }
    }
}
