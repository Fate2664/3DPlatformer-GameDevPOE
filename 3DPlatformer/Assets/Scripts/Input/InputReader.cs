using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Platformer
{
    //This is a script for reading the inputs from the player. It uses unity's input actions
    public class InputReader : MonoBehaviour, PlayerInputActions.IPlayerActions, PlayerInputActions.IUIActions
    {
        #region Class Variables

        [SerializeField] private bool holdToSprint = true;
        
        public PlayerInputActions inputActions { get; private set; }
        public Vector2 MovementInput {get ; private set;}
        public Vector2 LookInput {get ; private set;}
        public bool SprintToggledOn { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool NextPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool PausePressed { get; private set; }
        
        //UI Actions
        public event UnityAction<bool> Exit  =  delegate { };
        public event UnityAction<bool> RestoreDefaults  =  delegate { };
        public event UnityAction<bool> Apply  =  delegate { };
        public event UnityAction<float> VerticalNav  =  delegate { };
        public event UnityAction<float> HorizontalNav  =  delegate { };
        public event UnityAction<float> TabNav = delegate { };
        #endregion

        #region Startup & Update Methods

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
                inputActions.UI.SetCallbacks(this);
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
        
        //Reset boolean flags
        private void LateUpdate()
        {
            JumpPressed = false;
            NextPressed = false;
            InteractPressed = false;
            PausePressed = false;
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

        public void OnCrouch(InputAction.CallbackContext context)
        {
            return;
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

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            InteractPressed = true;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            PausePressed = true;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput =  context.ReadValue<Vector2>();
        }
        

        #endregion

        #region UI Inputs

        public void OnExit(InputAction.CallbackContext context)
        {
            Exit.Invoke(context.phase == InputActionPhase.Performed);
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            return;
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            return;
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            NextPressed = true;
        }

        public void OnRestoreDefaults(InputAction.CallbackContext context)
        {
            RestoreDefaults.Invoke(context.phase == InputActionPhase.Performed);
        }

        public void OnHorizontalNavigation(InputAction.CallbackContext context)
        {
            HorizontalNav.Invoke(context.ReadValue<float>());
        }

        public void OnVerticalNavigation(InputAction.CallbackContext context)
        {
            VerticalNav.Invoke(context.ReadValue<float>());
        }

        public void OnTabNavigation(InputAction.CallbackContext context)
        {
            TabNav.Invoke(context.ReadValue<float>());
        }

        public void OnApply(InputAction.CallbackContext context)
        {
            Apply.Invoke(context.phase == InputActionPhase.Performed);
        }

        #endregion

    }
}

