using System;
using Src.Audio;
using Src.Control;
using Src.Interaction.Death;
using Src.Interaction.Inventory;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Src.Interaction.Dialog.Orders
{
	public class Orders : MonoBehaviour, IInteractable
	{
		[SerializeField] private AudioClip _poisonTutorialClip;
		[SerializeField] private AudioClip _extraPoisonTutorialClip;

		[SerializeField] private AudioClip[] _poisonClip;

		[TextArea(3, 10)] [SerializeField] private string[] _poisonTutorialLines;
		[TextArea(3, 10)] [SerializeField] private string[] _poisonTutorialFinishLines;
		[TextArea(3, 10)] [SerializeField] private string[] _extraPoisonTutorialLines;
		[TextArea(3, 10)] [SerializeField] private string[] _extraPoisonTutorialFinishLines;
		[TextArea(3, 10)] [SerializeField] private string[] _failFinishLines;

		[SerializeField] private DialogueLinesGroup[] _poisonDialogues;
		[SerializeField] private DialogueLinesGroup[] _poisonFinishDialogues;
		[SerializeField] private DialogueLinesGroup[] _extraPoisonDialogues;
		[SerializeField] private DialogueLinesGroup[] _extraPoisonFinishDialogues;

		private OrderStates _orderState = OrderStates.NotTaken;
		private DialogManager _dialogManager;
		private bool _tutorialPoison;
		private bool _tutorialExtraPoison;
		private TopDownController _characterController;
		private Inventory.Inventory _inventory;
		private WitchDeath _witchDeath;
		private bool _orderType;
		private int _currentDialogId;
		private AudioClip _currentClip;
		private MusicManager _musicManager;

		private void Awake()
		{
			_dialogManager = FindAnyObjectByType<DialogManager>();
			_characterController = FindAnyObjectByType<TopDownController>();
			_inventory = FindAnyObjectByType<Inventory.Inventory>();
			_witchDeath = FindAnyObjectByType<WitchDeath>();
			_musicManager = FindAnyObjectByType<MusicManager>();
			_currentClip = GetRandomElement(_poisonClip);
		}

		public void Use()
		{
			if (FailureCheck())
			{
				return;
			}

			if (!_tutorialPoison)
			{
				PoisonTutorial();
				if (_orderState == OrderStates.NotTaken)
				{
					_characterController.CanControl = true;
				}

				return;
			}

			if (!_tutorialExtraPoison)
			{
				ExtraPoisonTutorial();
				if (_orderState == OrderStates.NotTaken)
				{
					_characterController.CanControl = true;
				}

				return;
			}

			CycleOrders();
		}

		private static T GetRandomElement<T>(T[] array)
		{
			if (array == null || array.Length == 0)
			{
				throw new ArgumentException("Массив пуст или null");
			}

			return array[Random.Range(0, array.Length)];
		}

		private bool FailureCheck()
		{
			switch (_orderState)
			{
				case OrderStates.NotTaken:
					return false;
				case OrderStates.Poison when _inventory.InventoryState == InventoryStates.ExtraPoison:
				case OrderStates.ExtraPoison when _inventory.InventoryState == InventoryStates.Poison:
					_musicManager.StopMusic();
					_dialogManager.OnDialogEnd += _witchDeath.Kill;
					_dialogManager.StartDialogue(_failFinishLines, _currentClip);
					return true;
				default:
					return false;
			}
		}

		private void PoisonTutorial()
		{
			switch (_tutorialPoison)
			{
				case false when _orderState == OrderStates.NotTaken:
					_currentClip = _poisonTutorialClip;
					_dialogManager.StartDialogue(_poisonTutorialLines, _poisonTutorialClip);
					_orderState = OrderStates.Poison;
					return;
				case false when _orderState == OrderStates.Poison &&
				                _inventory.InventoryState == InventoryStates.Poison:
					_inventory.ChangeSlot(InventoryStates.Empty);
					_dialogManager.StartDialogue(_poisonTutorialFinishLines, _poisonTutorialClip);
					_tutorialPoison = true;
					_orderState = OrderStates.NotTaken;
					return;
				case false:
					_characterController.CanControl = true;
					return;
			}
		}

		private void ExtraPoisonTutorial()
		{
			switch (_tutorialExtraPoison)
			{
				case false when _orderState == OrderStates.NotTaken:
					_currentClip = _extraPoisonTutorialClip;
					_dialogManager.StartDialogue(_extraPoisonTutorialLines, _extraPoisonTutorialClip);
					_orderState = OrderStates.ExtraPoison;
					return;
				case false when _orderState == OrderStates.ExtraPoison &&
				                _inventory.InventoryState == InventoryStates.ExtraPoison:
					_inventory.ChangeSlot(InventoryStates.Empty);
					_dialogManager.StartDialogue(_extraPoisonTutorialFinishLines, _extraPoisonTutorialClip);
					_tutorialExtraPoison = true;
					_orderState = OrderStates.NotTaken;
					return;
				case false:
					_characterController.CanControl = true;
					return;
			}
		}

		private void CycleOrders()
		{
			switch (_orderState)
			{
				case OrderStates.NotTaken:
				{
					_orderState = Random.value > 0.5f ? OrderStates.Poison : OrderStates.ExtraPoison;
					_currentClip = GetRandomElement(_poisonClip);

					switch (_orderState)
					{
						case OrderStates.Poison:
							_currentDialogId = Random.Range(0, _poisonDialogues.Length);
							_dialogManager.StartDialogue(_poisonDialogues[_currentDialogId].Lines, _currentClip);
							return;
						case OrderStates.ExtraPoison:
							_currentDialogId = Random.Range(0, _poisonFinishDialogues.Length);
							_dialogManager.StartDialogue(_extraPoisonDialogues[_currentDialogId].Lines, _currentClip);
							return;
						case OrderStates.NotTaken:
						default:
							return;
					}
				}
				case OrderStates.Poison when _inventory.InventoryState == InventoryStates.Poison:
					_inventory.ChangeSlot(InventoryStates.Empty);
					_dialogManager.StartDialogue(_poisonFinishDialogues[_currentDialogId].Lines, _currentClip);
					_orderState = OrderStates.NotTaken;
					return;
				case OrderStates.ExtraPoison when _inventory.InventoryState == InventoryStates.ExtraPoison:
					_inventory.ChangeSlot(InventoryStates.Empty);
					_dialogManager.StartDialogue(_extraPoisonFinishDialogues[_currentDialogId].Lines, _currentClip);
					_orderState = OrderStates.NotTaken;
					return;
				default:
					_characterController.CanControl = true;
					return;
			}
		}
	}
}