using System.Collections.Generic;
using Src.Control;
using Src.Interaction.Inventory;
using UnityEngine;

namespace Src.Interaction.Dialog.Orders
{
    public class Orders : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioClip _poisonTutorialClip;
        [SerializeField] private AudioClip _extraPoisonTutorialClip;
        [TextArea(3, 10)] [SerializeField] private string[] _poisonTutorialLines;
        [TextArea(3, 10)] [SerializeField] private string[] _poisonTutorialFinishLines;
        [TextArea(3, 10)] [SerializeField] private string[] _extraPoisonTutorialLines;
        [TextArea(3, 10)] [SerializeField] private string[] _extraPoisonTutorialFinishLines;
        [TextArea(3, 10)] [SerializeField] private List<string[]> _poisonVariantLines;

        private OrderStates _orderState = OrderStates.NotTaken;
        private DialogManager _dialogManager;
        private bool _tutorialPoison;
        private bool _tutorialExtraPoison;
        private TopDownController _characterController;
        private Inventory.Inventory _inventory;

        private void Awake()
        {
            _dialogManager = FindAnyObjectByType<DialogManager>();
            _characterController = FindAnyObjectByType<TopDownController>();
            _inventory = FindAnyObjectByType<Inventory.Inventory>();
        }

        public void Use()
        {
            if (!_tutorialPoison && _orderState == OrderStates.NotTaken)
            {
                _dialogManager.StartDialogue(_poisonTutorialLines, _poisonTutorialClip);
                _orderState = OrderStates.Poison;
                return;
            }

            if (!_tutorialPoison && _orderState == OrderStates.Poison &&
                _inventory.InventoryState == InventoryStates.Poison)
            {
                _inventory.ChangeSlot(InventoryStates.Empty);
                _dialogManager.StartDialogue(_poisonTutorialFinishLines, _poisonTutorialClip);
                _tutorialPoison = true;
                _orderState = OrderStates.NotTaken;
                return;
            }

            if (!_tutorialPoison)
            {
                _characterController.CanControl = true;
                return;
            }

            if (!_tutorialExtraPoison && _orderState == OrderStates.NotTaken)
            {
                _dialogManager.StartDialogue(_extraPoisonTutorialLines, _extraPoisonTutorialClip);
                _orderState = OrderStates.Poison;
                return;
            }

            if (!_tutorialExtraPoison && _orderState == OrderStates.Poison &&
                _inventory.InventoryState == InventoryStates.ExtraPoison)
            {
                _inventory.ChangeSlot(InventoryStates.Empty);
                _dialogManager.StartDialogue(_extraPoisonTutorialFinishLines, _extraPoisonTutorialClip);
                _tutorialExtraPoison = true;
                _orderState = OrderStates.NotTaken;
                return;
            }

            if (!_tutorialExtraPoison)
            {
                _characterController.CanControl = true;
                return;
            }

            _characterController.CanControl = true;
        }
    }
}