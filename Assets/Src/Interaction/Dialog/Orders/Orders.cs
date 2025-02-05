using UnityEngine;

namespace Src.Interaction.Dialog.Orders
{
    public class Orders : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioClip _textDialogClip;
        [TextArea(3, 10)] [SerializeField] private string[] _dialogueLines;

        private DialogManager _dialogManager;

        private void Awake()
        {
            _dialogManager = FindAnyObjectByType<DialogManager>();
        }

        public void Use()
        {
            _dialogManager.StartDialogue(_dialogueLines, _textDialogClip);
        }
    }
}