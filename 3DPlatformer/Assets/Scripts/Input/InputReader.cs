using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class InputReader : MonoBehaviour, PlayerInputActions.IPlayerActions, PlayerInputActions.IUIActions
    {
        #region Class Variables

        [SerializeField] private bool holdToSprint = true;
        
        public PlayerInputActions inputActions { get; private set; }
        public Vector2 MovementInput {get ; private set;}
        public Vector2 LookInput {get ; private set;}
        public bool SprintToggledOn { get; private set; }
        public bool JumpPressed { get; private set; }

        #endregion

        #region Startup & Update Methods

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
            inputActions.Enable();
        }
        
        private void OnDisable()
        {
            if (inputActions != null)
            {
                inputActions.Disable();
            }
        }

        private void LateUpdate()
        {
            JumpPressed = false;
        }
        #endregion

        #region Gameplay Inputs

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            return;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            JumpPressed = true;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SprintToggledOn = holdToSprint || !SprintToggledOn;
            }
            else if (context.canceled)
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput =  context.ReadValue<Vector2>();
        }
        

        #endregion

        #region UI Inputs

        public void OnExit(InputAction.CallbackContext context)
        {
            return;
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            return;
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            return;
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            return;
        }

        public void OnApply(InputAction.CallbackContext context)
        {
        }

        #endregion

    }
}

