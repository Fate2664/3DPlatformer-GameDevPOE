using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Platformer
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions, PlayerInputActions.IUIActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2> Look = delegate { };

        private PlayerInputActions inputActions;
        
        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
            inputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>());
        }

        public void OnExit(InputAction.CallbackContext context)
        {
        }

        public void OnApply(InputAction.CallbackContext context)
        {
        }
    }
}
