using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

namespace Src.Interaction.Dialog
{
    public class Dialog : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioClip _textDialogClip;
        [TextArea(3, 10)] [SerializeField] private string[] _dialogueLines;
        [SerializeField] private LocalizedString[] _localizedDialogueKeys;

        private DialogManager _dialogManager;

        private void Awake()
        {
            _dialogManager = FindAnyObjectByType<DialogManager>();
        }

        public void Use()
        {
            LoadLocalizedDialogue();
        }

        private static async Task<string> GetLocalizedStringAsync(LocalizedString localizedString)
        {
            var handle = localizedString.GetLocalizedStringAsync();
            while (!handle.IsDone)
            {
                await Task.Yield();
            }

            return handle.Result;
        }

        private async void LoadLocalizedDialogue()
        {
            if (_localizedDialogueKeys.Length != _dialogueLines.Length)
            {
                Debug.LogError(
                    $"[Dialog] Количество ключей ({_localizedDialogueKeys.Length}) не совпадает с количеством строк ({_dialogueLines.Length})!");
                return;
            }

            for (var i = 0; i < _localizedDialogueKeys.Length; i++)
            {
                _dialogueLines[i] = await GetLocalizedStringAsync(_localizedDialogueKeys[i]);
            }

            _dialogManager.StartDialogue(_dialogueLines, _textDialogClip);
        }
    }
}