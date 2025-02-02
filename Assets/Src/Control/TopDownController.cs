using UnityEngine;
using UnityEngine.InputSystem;

namespace Src.Control
{
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
            _playerInput = FindAnyObjectByType<PlayerInput>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _playerInput.onActionTriggered += ActionTriggered;
        }

        private void ActionTriggered(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.action.name == "Move")
            {
                Move(callbackContext.ReadValue<Vector2>().normalized);
            }
        }

        private void Move(Vector2 direction)
        {
            _rigidbody2D.linearVelocity = direction * RunSpeed;

            FlipSprite(direction);
            ChangeAnimationToRun(direction);
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