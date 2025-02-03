using System.Collections.Generic;
using Src.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Src.Interaction
{
    public class Interactor : MonoBehaviour
    {
        private readonly List<Collider2D> _triggers = new();

        [SerializeField] private GameObject _mark;

        private TopDownController _characterController;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _characterController = FindAnyObjectByType<TopDownController>();
        }

        private void OnEnable()
        {
            _playerInput = FindAnyObjectByType<PlayerInput>();
            _playerInput.actions["Interact"].started += UseInteractable;
        }

        private void OnDisable()
        {
            if (!_playerInput)
            {
                return;
            }

            _playerInput.actions["Interact"].started -= UseInteractable;
        }

        private void FixedUpdate()
        {
            _mark.SetActive(_triggers.Count != 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IInteractable _) && !_triggers.Contains(other))
            {
                _triggers.Add(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_triggers.Contains(other))
            {
                _triggers.Remove(other);
            }
        }

        private void UseInteractable(InputAction.CallbackContext callbackContext)
        {
            if (!_characterController.CanControl)
            {
                return;
            }

            if (_triggers.Count == 0)
            {
                return;
            }

            if (!_triggers[^1].TryGetComponent<IInteractable>(out var interactable))
            {
                return;
            }

            _characterController.CanControl = false;
            interactable.Use();
            _triggers.Remove(_triggers[^1]);
        }
    }
}