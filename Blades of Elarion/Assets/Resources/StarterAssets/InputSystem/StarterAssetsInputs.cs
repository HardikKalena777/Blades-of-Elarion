using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		WeaponManager weaponManager;
        ComboManager comboManager;

        [Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
        public bool draw;
		public bool sheath;
		public bool lightAttack;
		public bool heavyAttack;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        private void Awake()
        {
            weaponManager = GetComponent<WeaponManager>();
            comboManager = GetComponent<ComboManager>();
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
            if (value.isPressed)
            {
				weaponManager.HandleWeaponToggle();
            }
        }

        public void OnSheath(InputValue value)
        {
            SheathInput(value.isPressed);
            if (value.isPressed)
            {
                weaponManager.HandleWeaponToggle();
            }
        }

        public void OnLightAttack(InputValue value)
        {
            LightAttackInput(value.isPressed);
			if(value.isPressed)
			{
				comboManager.Attack();
            }
        }
        public void OnHeavyAttack(InputValue value)
        {
            HeavyAttackInput(value.isPressed);
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

        public void LightAttackInput(bool newLightAttackState)
		{
            lightAttack = newLightAttackState;
        }
        public void HeavyAttackInput(bool newHeavyAttackState)
        {
            heavyAttack = newHeavyAttackState;
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