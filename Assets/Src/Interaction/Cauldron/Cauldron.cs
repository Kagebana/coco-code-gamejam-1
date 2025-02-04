using Src.Control;
using UnityEngine;

namespace Src.Interaction.Cauldron
{
    public class Cauldron : MonoBehaviour, IInteractable
    {
        private TopDownController _characterController;
        private Inventory.Inventory _characterInventory;

        private void Awake()
        {
            _characterController = FindFirstObjectByType<TopDownController>();
            _characterInventory = FindFirstObjectByType<Inventory.Inventory>();
        }

        public void Use()
        {
            _characterController.CanControl = true;
        }
    }
}