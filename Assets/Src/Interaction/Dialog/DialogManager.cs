using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

        [SerializeField] private CanvasGroup _dialogCanvasGroup;
        [SerializeField] private TextMeshProUGUI _dialogText;

        private string[] _lines = { };
        private int _index;
        private bool _isTyping;
        private CancellationTokenSource _cancellationTokenSource;

        private TopDownController _characterController;
        private PlayerInput _playerInput;
        private AudioSource _audioSource;

        private void Awake()
        {
            _characterController = FindAnyObjectByType<TopDownController>();
            _playerInput = FindAnyObjectByType<PlayerInput>();
            _audioSource = GetComponent<AudioSource>();
        }

        public void StartDialogue(string[] lines, AudioClip textClip)
        {
            _lines = lines;
            _index = 0;

            CancelTypingTask();

            _dialogText.text = "";
            _audioSource.clip = textClip;

            _dialogCanvasGroup.alpha = 1;

            _playerInput.actions["Interact"].started += OnInteract;

            _cancellationTokenSource = new CancellationTokenSource();
            _ = TypeLineAsync(_cancellationTokenSource.Token);
        }

        private void OnInteract(InputAction.CallbackContext callbackContext)
        {
            if (_dialogCanvasGroup.alpha == 0)
            {
                return;
            }

            if (_isTyping)
            {
                CancelTypingTask();
                _dialogText.text = _lines[_index];
                _isTyping = false;
            }
            else
            {
                _index++;
                if (_index < _lines.Length)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _ = TypeLineAsync(_cancellationTokenSource.Token);
                }
                else
                {
                    _playerInput.actions["Interact"].started -= OnInteract;
                    _dialogCanvasGroup.alpha = 0;
                    _characterController.CanControl = true;
                    _index = 0;
                }
            }
        }

        private async Task TypeLineAsync(CancellationToken cancellationToken)
        {
            _isTyping = true;
            _dialogText.text = "";

            var rawText = _lines[_index];
            var visibleText = new StringBuilder();
            var matches = Regex.Matches(rawText, @"(<.*?>)|([^<]+)");

            foreach (Match match in matches)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _isTyping = false;
                    return;
                }

                var part = match.Value;

                if (part.StartsWith("<"))
                {
                    visibleText.Append(part);
                }
                else
                {
                    foreach (var c in part)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _isTyping = false;
                            return;
                        }

                        visibleText.Append(c);
                        _dialogText.text = visibleText.ToString();
                        _audioSource.Play();

                        try
                        {
                            await Task.Delay((int)(TextSpeed * 1000), cancellationToken);
                        }
                        catch (TaskCanceledException)
                        {
                            _isTyping = false;
                            return;
                        }
                    }
                }
            }

            _isTyping = false;
        }

        private void CancelTypingTask()
        {
            if (_cancellationTokenSource == null)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}