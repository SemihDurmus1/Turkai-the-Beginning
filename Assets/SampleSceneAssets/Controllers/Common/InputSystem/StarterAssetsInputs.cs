using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool attack;
		public bool weapon;
		public bool interact;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Inventory Locker")]
        public bool isInventoryOpen = false;

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

		public void OnAttack(InputValue value)//Modifiye edilmeli
		{
            if (!isInventoryOpen) // Eger envanter acik degilse saldiri yapmaya able ol
            {
                AttackInput(value.isPressed);
            }

			//Zamani gelince envanter sorgusunu kaldirmayi dusunuyorum
        }

		public void OnWeaponChoose(InputValue value)
		{
			ChooseWeaponInput(value.isPressed);
            Debug.Log("OnWeaponChoose calisti");
        }

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
            Debug.Log("InteractInput calisti");
        }



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

		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;
		}

        public void ChooseWeaponInput(bool newchooseWeaponState)
		{
			weapon = newchooseWeaponState;
			Debug.Log("ChooseWeaponInput calisti");

        }

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
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
