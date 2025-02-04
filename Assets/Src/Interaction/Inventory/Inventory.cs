using UnityEngine;

namespace Src.Interaction.Inventory
{
    [RequireComponent(typeof(AudioSource))]
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _slot;
        [SerializeField] private Sprite[] _ingredients;
        [SerializeField] private Sprite[] _flasks;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public InventoryStates InventoryStates { get; private set; } = InventoryStates.Empty;

        public void ChangeSlot(InventoryStates inventoryState)
        {
            InventoryStates = inventoryState;

            _slot.sprite = InventoryStates switch
            {
                InventoryStates.Empty => null,
                InventoryStates.White => _ingredients[0],
                InventoryStates.Red => _ingredients[1],
                InventoryStates.Green => _ingredients[2],
                InventoryStates.Purple => _ingredients[3],
                InventoryStates.Flask => _flasks[0],
                InventoryStates.Poison => _flasks[1],
                InventoryStates.ExtraPoison => _flasks[2],
                _ => _slot.sprite
            };

            _audioSource.Play();
        }
    }
}