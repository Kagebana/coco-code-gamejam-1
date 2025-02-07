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
		private Tutorial.Tutorial _tutorial;

		private void Awake()
		{
			_characterController = FindFirstObjectByType<TopDownController>();
			_characterInventory = FindFirstObjectByType<Inventory.Inventory>(FindObjectsInactive.Include);
			_tutorial = FindAnyObjectByType<Tutorial.Tutorial>();
		}

		public void Use()
		{
			_characterController.CanControl = true;

			if (_characterInventory.InventoryState is InventoryStates.Poison or InventoryStates.ExtraPoison)
			{
				return;
			}

			_characterInventory.ChangeSlot(_inventoryState);
			_tutorial.ChangeToCauldron();
		}
	}
}