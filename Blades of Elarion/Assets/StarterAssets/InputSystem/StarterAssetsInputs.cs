using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		WeaponHandler weaponHandler;
		CombatSystem combatSystem;
		StarterAssetsInputs inputs;
		DodgeRoll dodgeRoll;
		UIManager uiManager;

        [Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool draw;
		public bool sheath;
		public bool attack;
		public bool dodge;
		public bool toggleUI;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;


        private void Start()
        {
			inputs = GetComponent<StarterAssetsInputs>();
			weaponHandler = GetComponent<WeaponHandler>();
			combatSystem = GetComponent<CombatSystem>();
			dodgeRoll = GetComponent<DodgeRoll>();
			uiManager = GetComponent<UIManager>();
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnDraw(InputValue value)
		{
			DrawInput(value.isPressed);
			if(value.isPressed)
			{
				weaponHandler.ToggleWeapon();
			}
		}

        public void OnSheath(InputValue value)
        {
            SheathInput(value.isPressed);
            if (value.isPressed)
            {
                weaponHandler.ToggleWeapon();
            }
        }

		public void OnAttack(InputValue value)
		{
			AttackInput(value.isPressed);
			if(value.isPressed)
			{
				combatSystem.HandleAttackInput();
			}
		}

		public void OnDodge(InputValue value)
		{
			DodgeInput(value.isPressed);
			if(value.isPressed)
			{
				dodgeRoll.HandleRollInput();
			}
		}

		public void OnToggleUI(InputValue value)
		{
			ToggleUIInput(value.isPressed);
			if(value.isPressed)
			{
				uiManager.ToggleUI();
			}
        }

#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void DrawInput(bool newDrawState)
		{
			draw = newDrawState;
		}

		public void SheathInput(bool newSheathState)
		{
			sheath = newSheathState;
		}

		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;
		}

		public void DodgeInput(bool newDodgeState)
		{
			dodge = newDodgeState;
		}

		public void ToggleUIInput(bool newToggleUIState)
		{
			toggleUI = newToggleUIState;
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}