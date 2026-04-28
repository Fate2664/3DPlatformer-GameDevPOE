using UnityEngine;

namespace Platformer
{
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
