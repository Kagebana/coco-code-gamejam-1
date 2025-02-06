using Src.Control;
using Src.Interaction.Inventory;
using UnityEngine;

namespace Src.Interaction.Cauldron
{
    public class Ingredient : MonoBehaviour, IInteractable
    {
        [SerializeField] private InventoryStates _inventoryState = InventoryStates.White;

        private TopDownController _characterController;
        private Inventory.Inventory _characterInventory;

        private void Awake()
        {
            _characterController = FindFirstObjectByType<TopDownController>();
            _characterInventory = FindFirstObjectByType<Inventory.Inventory>(FindObjectsInactive.Include);
        }

        public void Use()
        {
            _characterController.CanControl = true;
            _characterInventory.ChangeSlot(_inventoryState);
        }
    }
}