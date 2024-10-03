using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerInput))]
	public class FirstPersonController : MonoBehaviour
	{
        #region EditorVariables
        [Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;
        #endregion

        // cinemachine
        private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

        private bool isAttacking = false;

		private PlayerInput _playerInput;
		private Animator _playerAnimator;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		private AttackSystem _playerAttackSystem;
		private DashDodge dashDodge;

        //[SerializeField] private float pickupRange = 2f;
        //[SerializeField] private Transform playerCameraRoot;

        private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				return _playerInput.currentControlScheme == "KeyboardMouse";
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();

			_input = GetComponent<StarterAssetsInputs>();

			_playerInput = GetComponent<PlayerInput>();

			_playerAnimator = GetComponent<Animator>();

			_playerAttackSystem = GetComponentInChildren<AttackSystem>();

            dashDodge = GetComponent<DashDodge>();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

            //playerCameraRoot = GameObject.FindGameObjectWithTag("PlayerCameraRoot").transform;

        }

        private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
            CheckAttack();
			ToggleInventory();
			//RaycastForInteract();
        }

		private void LateUpdate()
		{
			CameraRotation();
		}

        

        private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			float targetSpeed;

            // set target speed based on move speed, sprint speed and if sprint is pressed
            if (_input.sprint)
			{
                targetSpeed = SprintSpeed;
				_playerAnimator.SetBool("isSprinting", true);
            }
			else
			{
				targetSpeed = MoveSpeed;
				_playerAnimator.SetBool("isSprinting", false);
            }
			

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            _playerAnimator.SetFloat("speed", _speed);
		 
			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}


			//Manage the move animations
			_playerAnimator.SetFloat("YatayEksen", _input.move.x);
            _playerAnimator.SetFloat("DikeyEksen", _input.move.y);

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (dashDodge.isDodging) { return; }

			if (Grounded && dashDodge.isDodging == false && dashDodge.dodgeInputDetected == false)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{

                _verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void CheckAttack()//Need to develop attack
		{
			if (_input.attack && !isAttacking)
			{
				_playerAnimator.SetTrigger("Attack");

				isAttacking = true;
                _input.attack = false;
            }
		}

        void OnAttackAnimationStart()//Animation Event
        {
            if (_playerAttackSystem == null)
            {
                _playerAttackSystem = GetComponentInChildren<AttackSystem>();
            }
            //_playerAttackSystem = PlayerManager.Instance.onHandSlot.GetComponentInChildren<AttackSystem>();

            if (_playerAttackSystem != null)
            {
                _playerAttackSystem.StartAttack();
            }
        }
        void OnAttackAnimationEnd()//Animation Event
        {
            if (_playerAttackSystem != null)
            {
                _playerAttackSystem.EndAttack();
                //isAttacking = false;
            }
        }

		void CanAttackAgain()
		{
            isAttacking = false;
        }

        private void ToggleInventory()
		{
            if (Input.GetKeyDown(KeyCode.C))//Buran�n new input sisteme uyarlanmasi gerekiyor
            {
                InventoryUI.Instance.inventoryPanel.SetActive(!InventoryUI.Instance.inventoryPanel.activeSelf);

                StarterAssetsInputs playerInput = FindFirstObjectByType<StarterAssetsInputs>();
                playerInput.isInventoryOpen = InventoryUI.Instance.inventoryPanel.activeSelf; // Envanter a��ksa input'u kapat

                CursorManager.Instance.SetCursorState(!InventoryUI.Instance.inventoryPanel.activeSelf);
				//HandFirstItem();
			}
		}

        #region Raycast prototip

		//Bu kodlar ItemCollect scriptindeki raycast collecti buraya uyarlamak icin deneme idi,
		//cunku raycast yapinca birden fazla item alabiliyor ayni anda, onu cozmem lazim
		//ama simdilik olmadi


        //private void RaycastForInteract()
        //{
        //	if (Input.GetKeyDown(KeyCode.F))
        //	{
        //		Debug.Log("F ye basildi interact calisti");
        //		CollectNearestItem();
        //	}
        //}

        //      private void CollectNearestItem()
        //      {
        //          Ray ray = new Ray(playerCameraRoot.position, playerCameraRoot.forward * pickupRange);
        //          RaycastHit hit;

        //          Debug.DrawRay(ray.origin, ray.direction, Color.red);

        //          if (Physics.Raycast(ray, out hit, pickupRange, ~0, QueryTriggerInteraction.Collide))
        //          {
        //              ItemCollect item = hit.collider.gameObject.GetComponentInParent<ItemCollect>();

        //              if (hit.collider.gameObject.transform.root == gameObject.transform.root)
        //              {


        //                  InventorySystem.Instance.AddItem(item.itemReference, 1);
        //                  InventoryUI.Instance.RefreshUI();

        //                  ////collectCanvasPrefab.SetActive(false);
        //                  //if (collectCanvasInstance != null)
        //                  //{
        //                  //    Destroy(collectCanvasInstance);
        //                  //}
        //                  Destroy(hit.collider.gameObject);
        //              }
        //          }
        //          else
        //          {
        //              Debug.Log("Ray hicbir seye carpmadi");
        //          }
        //      }
        #endregion

        private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}
