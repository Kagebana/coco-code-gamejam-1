using System.Collections.Generic;
using System.Linq;
using Src.Control;
using Src.Interaction.Death;
using Src.Interaction.Inventory;
using UnityEngine;

namespace Src.Interaction.Cauldron
{
    public class Cauldron : MonoBehaviour, IInteractable
    {
        private static readonly int s_baseIngredient = Animator.StringToHash("BaseIngredient");
        private static readonly int s_addedIngredient = Animator.StringToHash("AddedIngredient");
        private static readonly int s_poison = Animator.StringToHash("Poison");
        private static readonly int s_superPoison = Animator.StringToHash("SuperPoison");
        private static readonly int s_potionTaken = Animator.StringToHash("PotionTaken");

        private readonly List<InventoryStates> _currentIngredients = new();

        [SerializeField] private Animator _animator;

        private CauldronStates _cauldronState = CauldronStates.Wait;
        private TopDownController _characterController;
        private Inventory.Inventory _characterInventory;
        private WitchDeath _characterDeath;

        private readonly InventoryStates[] _poisonRecipe =
            { InventoryStates.Red, InventoryStates.Purple, InventoryStates.Green, InventoryStates.Green };

        private readonly InventoryStates[] _extraPoisonRecipe =
        {
            InventoryStates.Purple, InventoryStates.Red, InventoryStates.Red, InventoryStates.Purple,
            InventoryStates.Green
        };

        private void Awake()
        {
            _characterController = FindFirstObjectByType<TopDownController>();
            _characterInventory = FindFirstObjectByType<Inventory.Inventory>();
            _characterDeath = FindFirstObjectByType<WitchDeath>();
        }

        private bool CanUse { get; set; }

        public void AllowUse()
        {
            CanUse = true;
        }

        public void Use()
        {
            _characterController.CanControl = true;

            if (!CanUse)
            {
                return;
            }

            if (_cauldronState == CauldronStates.Potion)
            {
                TakePotion();
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
                    _animator.SetTrigger(s_baseIngredient);
                    _cauldronState = CauldronStates.Ready;
                    return;
                }
            }

            if (_cauldronState == CauldronStates.Ready)
            {
                AddIngredient(_characterInventory.InventoryState);
                _characterInventory.ChangeSlot(InventoryStates.Empty);
            }
        }

        private void AddIngredient(InventoryStates ingredient)
        {
            _currentIngredients.Add(ingredient);

            if (_currentIngredients.Count > Mathf.Max(_poisonRecipe.Length, _extraPoisonRecipe.Length))
            {
                _characterDeath.Kill();
                return;
            }

            if (CheckRecipe(_poisonRecipe))
            {
                if (_currentIngredients.Count == _poisonRecipe.Length)
                {
                    _animator.SetTrigger(s_poison);
                    _cauldronState = CauldronStates.Potion;
                    return;
                }
            }
            else if (CheckRecipe(_extraPoisonRecipe))
            {
                if (_currentIngredients.Count == _extraPoisonRecipe.Length)
                {
                    _animator.SetTrigger(s_superPoison);
                    _cauldronState = CauldronStates.Potion;
                    return;
                }
            }
            else
            {
                _characterDeath.Kill();
                return;
            }

            _animator.SetTrigger(s_addedIngredient);
        }

        private void TakePotion()
        {
            if (_characterInventory.InventoryState != InventoryStates.Flask)
            {
                return;
            }

            _characterInventory.ChangeSlot(
                _currentIngredients.Count == _poisonRecipe.Length
                    ? InventoryStates.Poison
                    : InventoryStates.ExtraPoison);

            _animator.SetTrigger(s_potionTaken);
            _currentIngredients.Clear();
            _cauldronState = CauldronStates.Wait;
        }

        private bool CheckRecipe(InventoryStates[] recipe)
        {
            if (_currentIngredients.Count > recipe.Length)
            {
                return false;
            }

            return !_currentIngredients.Where((t, i) => t != recipe[i]).Any();
        }
    }
}