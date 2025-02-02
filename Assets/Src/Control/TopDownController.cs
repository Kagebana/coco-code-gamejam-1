using System;
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
        private Vector2 _direction;

        private void Awake()
        {
            _playerInput = FindAnyObjectByType<PlayerInput>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _playerInput.onActionTriggered += ActionTriggered;
        }
        
        private void OnDisable()
        {
            _playerInput.onActionTriggered -= ActionTriggered;
        }

        private void FixedUpdate()
        {
            _rigidbody2D.linearVelocity = _direction * RunSpeed;

            FlipSprite(_direction);
            ChangeAnimationToRun(_direction);
        }

        private void ActionTriggered(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.action.name == "Move")
            {
                _direction = callbackContext.ReadValue<Vector2>().normalized;
            }
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