using System;
using Src.Interaction.Dialog.Orders;
using Src.Interaction.Inventory;
using Src.UI;
using UnityEngine;

namespace Src.Interaction.Tutorial
{
	public class Tutorial : MonoBehaviour
	{
		[SerializeField] private GameObject _navigation;
		[SerializeField] private GameObject _order, _flask, _cauldron, _gold, _red, _green, _purple;

		private TutorialStates _tutorialState = TutorialStates.Disable;
		private Menu _menu;
		private Enter.Enter _enter;
		private Orders _orders;
		private Cauldron.Cauldron _cauldronUsed;
		private int _ingredientId;
		private Transform _lookTarget;
		private bool _tutorialStep;
		private Inventory.Inventory _characterInventory;

		private Action _onStartedHandler, _onPoisonTutorialStartedHandler, _onPoisonTutorialFinishedHandler;
		private Action _onExtraPoisonTutorialStartedHandler, _onExtraPoisonTutorialFinishedHandler;
		private Action _onIngredientAddedHandler, _onPoisonBrewedHandler, _onPoisonTakenHandler;

		private void Awake()
		{
			_menu = FindAnyObjectByType<Menu>();
			_enter = FindAnyObjectByType<Enter.Enter>();
			_orders = FindAnyObjectByType<Orders>();
			_cauldronUsed = FindAnyObjectByType<Cauldron.Cauldron>();
			_characterInventory = FindAnyObjectByType<Inventory.Inventory>(FindObjectsInactive.Include);
		}

		private void OnEnable()
		{
			SubscribeToEvents();
		}

		private void OnDisable()
		{
			UnsubscribeFromEvents();
		}

		private void FixedUpdate()
		{
			if (!_menu.SkipTutorial)
			{
				LookAtTarget();
			}
		}

		private void SubscribeToEvents()
		{
			_enter.OnStarted += _onStartedHandler = () => ChangeState(TutorialStates.Order);
			_orders.OnPoisonTutorialStarted += _onPoisonTutorialStartedHandler = () => ChangeState(TutorialStates.Gold);
			_orders.OnPoisonTutorialFinished += _onPoisonTutorialFinishedHandler = () =>
			{
				_tutorialStep = true;
				_ingredientId = 0;
				ChangeState(TutorialStates.Order);
			};
			_orders.OnExtraPoisonTutorialStarted +=
				_onExtraPoisonTutorialStartedHandler = () => ChangeState(TutorialStates.Gold);
			_orders.OnExtraPoisonTutorialFinished +=
				_onExtraPoisonTutorialFinishedHandler = () => ChangeState(TutorialStates.Disable);
			_cauldronUsed.OnIngredientAdded += _onIngredientAddedHandler = ChangeByIngredient;
			_cauldronUsed.OnPoisonBrewed += _onPoisonBrewedHandler = () => ChangeState(TutorialStates.Flask);
			_cauldronUsed.OnPoisonTaken += _onPoisonTakenHandler = () => ChangeState(TutorialStates.Order);
		}

		private void UnsubscribeFromEvents()
		{
			_enter.OnStarted -= _onStartedHandler;
			_orders.OnPoisonTutorialStarted -= _onPoisonTutorialStartedHandler;
			_orders.OnPoisonTutorialFinished -= _onPoisonTutorialFinishedHandler;
			_orders.OnExtraPoisonTutorialStarted -= _onExtraPoisonTutorialStartedHandler;
			_orders.OnExtraPoisonTutorialFinished -= _onExtraPoisonTutorialFinishedHandler;
			_cauldronUsed.OnIngredientAdded -= _onIngredientAddedHandler;
			_cauldronUsed.OnPoisonBrewed -= _onPoisonBrewedHandler;
			_cauldronUsed.OnPoisonTaken -= _onPoisonTakenHandler;
		}

		public void ChangeToCauldron()
		{
			if (_menu.SkipTutorial)
			{
				return;
			}

			switch (_tutorialState)
			{
				case TutorialStates.Order:
				case TutorialStates.Gold when _characterInventory.InventoryState != InventoryStates.White:
					return;
				default:
					ChangeState(TutorialStates.Cauldron);
					break;
			}
		}

		private void ChangeState(TutorialStates newState)
		{
			if (_menu.SkipTutorial && newState != TutorialStates.Disable)
			{
				return;
			}

			if (newState == TutorialStates.Gold && _characterInventory.InventoryState == InventoryStates.White)
			{
				newState = TutorialStates.Cauldron;
			}

			_tutorialState = newState;
			_navigation.SetActive(true);
			DisableAllSteps();

			if (_tutorialState == TutorialStates.Disable)
			{
				_navigation.SetActive(false);
				return;
			}

			GameObject activeStep = GetActiveStep(_tutorialState);
			if (activeStep != null)
			{
				activeStep.SetActive(true);
			}

			_lookTarget = activeStep?.transform;
		}

		private void DisableAllSteps()
		{
			_order.SetActive(false);
			_flask.SetActive(false);
			_cauldron.SetActive(false);
			_gold.SetActive(false);
			_red.SetActive(false);
			_green.SetActive(false);
			_purple.SetActive(false);
		}

		private GameObject GetActiveStep(TutorialStates state)
		{
			return state switch
			{
				TutorialStates.Order => _order,
				TutorialStates.Flask => _flask,
				TutorialStates.Cauldron => _cauldron,
				TutorialStates.Gold => _gold,
				TutorialStates.Red => _red,
				TutorialStates.Green => _green,
				TutorialStates.Purple => _purple,
				_ => null
			};
		}

		private void ChangeByIngredient()
		{
			if (_menu.SkipTutorial)
			{
				return;
			}

			_ingredientId++;

			if (_tutorialStep)
			{
				ChangeState(_ingredientId switch
				{
					1 => TutorialStates.Purple,
					2 => TutorialStates.Red,
					3 => TutorialStates.Red,
					4 => TutorialStates.Purple,
					5 => TutorialStates.Green,
					_ => _tutorialState
				});
			}
			else
			{
				ChangeState(_ingredientId switch
				{
					1 => TutorialStates.Red,
					2 => TutorialStates.Purple,
					3 or 4 => TutorialStates.Green,
					_ => _tutorialState
				});
			}
		}

		private void LookAtTarget()
		{
			if (_lookTarget == null)
			{
				return;
			}

			Vector3 direction = _lookTarget.position - _navigation.transform.position;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			_navigation.transform.rotation = Quaternion.Euler(0, 0, angle);
		}
	}
}