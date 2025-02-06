using UnityEngine;
using UnityEngine.InputSystem;

namespace Src.Control
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class TopDownController : MonoBehaviour
	{
		private static readonly int Moving = Animator.StringToHash("Moving");

		private const float RunSpeed = 3;

		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private Animator _animator;

		private PlayerInput _playerInput;
		private Rigidbody2D _rigidbody2D;

		private void Awake()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_rigidbody2D = GetComponent<Rigidbody2D>();
		}

		private void OnEnable()
		{
			_playerInput = FindAnyObjectByType<PlayerInput>();
			_playerInput.actions["Move"].performed += Move;
			_playerInput.actions["Move"].canceled += Move;
		}

		private void OnDisable()
		{
			if (!_playerInput)
			{
				return;
			}

			_playerInput.actions["Move"].performed -= Move;
			_playerInput.actions["Move"].canceled -= Move;
		}

		private void FixedUpdate()
		{
			_rigidbody2D.linearVelocity = Direction * RunSpeed;

			FlipSprite(Direction);
			ChangeAnimationToRun(Direction);
		}

		public bool CanControl { get; set; }
		public Vector2 Direction { get; set; } = Vector2.zero;

		private void Move(InputAction.CallbackContext callbackContext)
		{
			if (!CanControl)
			{
				Direction = Vector2.zero;
				return;
			}

			Direction = callbackContext.ReadValue<Vector2>().normalized;
		}

		private void FlipSprite(Vector2 direction)
		{
			_spriteRenderer.flipX = direction.x switch
			{
				< 0 when !_spriteRenderer.flipX => true,
				> 0 when _spriteRenderer.flipX => false,
				_ => _spriteRenderer.flipX
			};
		}

		private void ChangeAnimationToRun(Vector2 direction)
		{
			_animator.SetBool(Moving, direction != Vector2.zero);
		}
	}
}