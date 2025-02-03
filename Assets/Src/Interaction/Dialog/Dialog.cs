using UnityEngine;

namespace Src.Interaction.Dialog
{
    public class Dialog : MonoBehaviour, IInteractable
    {
        [TextArea(3, 10)] [SerializeField] private string[] _dialogueLines;

        private DialogManager _dialogManager;

        private void Awake()
        {
            _dialogManager = FindAnyObjectByType<DialogManager>();
        }

        public void Use()
        {
            _dialogManager.StartDialogue(_dialogueLines);
        }
    }
}