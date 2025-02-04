using Src.Control;
using Src.Interaction.Inventory;
using UnityEngine;

namespace Src.Interaction.Cauldron
{
    public class Cauldron : MonoBehaviour, IInteractable
    {
        private static readonly int BaseIngredient = Animator.StringToHash("BaseIngredient");
        private static readonly int AddedIngredient = Animator.StringToHash("AddedIngredient");
        private static readonly int Poison = Animator.StringToHash("Poison");
        private static readonly int SuperPoison = Animator.StringToHash("SuperPoison");
        private static readonly int PotionTaken = Animator.StringToHash("PotionTaken");

        [SerializeField] private Animator _animator;

        private CauldronStates _cauldronState = CauldronStates.Wait;

        private TopDownController _characterController;
        private Inventory.Inventory _characterInventory;

        private void Awake()
        {
            _characterController = FindFirstObjectByType<TopDownController>();
            _characterInventory = FindFirstObjectByType<Inventory.Inventory>();
        }

        public bool CanUse { get; private set; }

        public void Use()
        {
            _characterController.CanControl = true;

            if (!CanUse)
            {
                return;
            }

            if (_characterInventory.InventoryState is InventoryStates.Empty or InventoryStates.Poison
                or InventoryStates.ExtraPoison)
            {
                return;
            }

            if (_cauldronState == CauldronStates.Wait)
            {
                if (_characterInventory.InventoryState == InventoryStates.White)
                {
                    CanUse = false;
                    _characterInventory.ChangeSlot(InventoryStates.Empty);
                    _animator.SetTrigger(BaseIngredient);
                    _cauldronState = CauldronStates.Ready;
                }
            }

            if (_cauldronState == CauldronStates.Ready)
            {
            }
        }

        public void AllowUse()
        {
            CanUse = true;
        }
    }
}