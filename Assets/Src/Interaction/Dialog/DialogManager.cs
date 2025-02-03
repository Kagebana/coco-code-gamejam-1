using System.Collections;
using Src.Control;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Src.Interaction.Dialog
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogManager : MonoBehaviour
    {
        private const float TextSpeed = 0.05f;

        [SerializeField] private GameObject _dialogBox;
        [SerializeField] private TextMeshProUGUI _dialogText;

        private string[] _lines = { };
        private int _index;
        private bool _isTyping;
        private Coroutine _typingCoroutine;

        private TopDownController _characterController;
        private PlayerInput _playerInput;
        private AudioSource _audioSource;

        private void Awake()
        {
            _characterController = FindAnyObjectByType<TopDownController>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _playerInput = FindAnyObjectByType<PlayerInput>();
            _playerInput.actions["Interact"].started += OnInteract;
        }

        private void OnDisable()
        {
            if (!_playerInput)
            {
                return;
            }

            _playerInput.actions["Interact"].started -= OnInteract;
        }

        public void StartDialogue(string[] lines, AudioClip textClip)
        {
            _dialogBox.SetActive(true);
            _lines = lines;
            _index = 0;

            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            _dialogText.text = "";
            _audioSource.clip = textClip;
            _typingCoroutine = StartCoroutine(TypeLine());
        }

        private void OnInteract(InputAction.CallbackContext callbackContext)
        {
            if (!_dialogBox.activeSelf)
            {
                return;
            }

            if (_isTyping)
            {
                StopCoroutine(_typingCoroutine);
                _dialogText.text = _lines[_index];
                _isTyping = false;
            }
            else
            {
                _index++;
                if (_index < _lines.Length)
                {
                    _typingCoroutine = StartCoroutine(TypeLine());
                }
                else
                {
                    _dialogBox.SetActive(false);
                    _characterController.CanControl = true;
                    _index = 0;
                }
            }
        }

        private IEnumerator TypeLine()
        {
            _isTyping = true;
            _dialogText.text = "";

            foreach (var c in _lines[_index])
            {
                _dialogText.text += c;
                _audioSource.Play();
                yield return new WaitForSeconds(TextSpeed);
            }

            _isTyping = false;
        }
    }
}