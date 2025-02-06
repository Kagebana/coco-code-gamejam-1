using System;
using Src.Interaction.Dialog.Orders;
using Src.UI;
using UnityEngine;

namespace Src.Interaction.Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private GameObject _order;
        [SerializeField] private GameObject _flask;
        [SerializeField] private GameObject _cauldron;
        [SerializeField] private GameObject _gold;
        [SerializeField] private GameObject _red;
        [SerializeField] private GameObject _green;
        [SerializeField] private GameObject _purple;

        private TutorialStates _tutorialState = TutorialStates.Disable;
        private Menu _menu;
        private Enter.Enter _enter;
        private Orders _orders;
        private Cauldron.Cauldron _cauldronUsed;
        private int _ingredientId;

        private bool _tutorialStep;

        private Action _onStartedHandler;
        private Action _onPoisonTutorialStartedHandler;
        private Action _onPoisonTutorialFinishedHandler;
        private Action _onExtraPoisonTutorialStartedHandler;
        private Action _onExtraPoisonTutorialFinishedHandler;

        private Action _onIngredientAddedHandler;
        private Action _onPoisonBrewedHandler;
        private Action _onPoisonTakenHandler;

        private void Awake()
        {
            _menu = FindAnyObjectByType<Menu>();
            _enter = FindAnyObjectByType<Enter.Enter>();
            _orders = FindAnyObjectByType<Orders>();
            _cauldronUsed = FindAnyObjectByType<Cauldron.Cauldron>();
        }

        private void OnEnable()
        {
            _onStartedHandler = () => ChangeState(TutorialStates.Order);
            _enter.OnStarted += _onStartedHandler;
            _onPoisonTutorialStartedHandler = () => ChangeState(TutorialStates.Gold);
            _orders.OnPoisonTutorialStarted += _onPoisonTutorialStartedHandler;
            _onPoisonTutorialFinishedHandler = () =>
            {
                _tutorialStep = true;
                _ingredientId = 0;
            };
            _orders.OnPoisonTutorialFinished += _onPoisonTutorialFinishedHandler;
            _onExtraPoisonTutorialStartedHandler = () => ChangeState(TutorialStates.Gold);
            _orders.OnExtraPoisonTutorialStarted += _onExtraPoisonTutorialStartedHandler;
            _onExtraPoisonTutorialFinishedHandler = () => ChangeState(TutorialStates.Disable);
            _orders.OnExtraPoisonTutorialFinished += _onExtraPoisonTutorialFinishedHandler;

            _onIngredientAddedHandler = ChangeByIngredient;
            _cauldronUsed.OnIngredientAdded += _onIngredientAddedHandler;

            _onPoisonBrewedHandler = () => ChangeState(TutorialStates.Flask);
            _cauldronUsed.OnPoisonBrewed += _onPoisonBrewedHandler;

            _onPoisonTakenHandler = () => ChangeState(TutorialStates.Order);
            _cauldronUsed.OnPoisonTaken += _onPoisonTakenHandler;
        }

        private void OnDisable()
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

            ChangeState(TutorialStates.Cauldron);
        }

        private void ChangeState(TutorialStates newState)
        {
            if (_menu.SkipTutorial)
            {
                return;
            }

            _tutorialState = newState;

            switch (_tutorialState)
            {
                case TutorialStates.Order:
                    _order.SetActive(true);
                    _flask.SetActive(false);
                    _cauldron.SetActive(false);
                    _gold.SetActive(false);
                    _red.SetActive(false);
                    _green.SetActive(false);
                    _purple.SetActive(false);
                    break;
                case TutorialStates.Flask:
                    _order.SetActive(false);
                    _flask.SetActive(true);
                    _cauldron.SetActive(false);
                    _gold.SetActive(false);
                    _red.SetActive(false);
                    _green.SetActive(false);
                    _purple.SetActive(false);
                    break;
                case TutorialStates.Cauldron:
                    _order.SetActive(false);
                    _flask.SetActive(false);
                    _cauldron.SetActive(true);
                    _gold.SetActive(false);
                    _red.SetActive(false);
                    _green.SetActive(false);
                    _purple.SetActive(false);
                    break;
                case TutorialStates.Gold:
                    _order.SetActive(false);
                    _flask.SetActive(false);
                    _cauldron.SetActive(false);
                    _gold.SetActive(true);
                    _red.SetActive(false);
                    _green.SetActive(false);
                    _purple.SetActive(false);
                    break;
                case TutorialStates.Red:
                    _order.SetActive(false);
                    _flask.SetActive(false);
                    _cauldron.SetActive(false);
                    _gold.SetActive(false);
                    _red.SetActive(true);
                    _green.SetActive(false);
                    _purple.SetActive(false);
                    break;
                case TutorialStates.Green:
                    _order.SetActive(false);
                    _flask.SetActive(false);
                    _cauldron.SetActive(false);
                    _gold.SetActive(false);
                    _red.SetActive(false);
                    _green.SetActive(true);
                    _purple.SetActive(false);
                    break;
                case TutorialStates.Purple:
                    _order.SetActive(false);
                    _flask.SetActive(false);
                    _cauldron.SetActive(false);
                    _gold.SetActive(false);
                    _red.SetActive(false);
                    _green.SetActive(false);
                    _purple.SetActive(true);
                    break;
                case TutorialStates.Disable:
                    _order.SetActive(false);
                    _flask.SetActive(false);
                    _cauldron.SetActive(false);
                    _gold.SetActive(false);
                    _red.SetActive(false);
                    _green.SetActive(false);
                    _purple.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeByIngredient()
        {
            if (_menu.SkipTutorial)
            {
                return;
            }
            print("Ingredient");
            _ingredientId++;
            if (!_tutorialStep)
            {
                if (_ingredientId == 1)
                {
                    ChangeState(TutorialStates.Red);
                }

                if (_ingredientId == 2)
                {
                    ChangeState(TutorialStates.Purple);
                }

                if (_ingredientId == 3)
                {
                    ChangeState(TutorialStates.Green);
                }

                if (_ingredientId == 4)
                {
                    ChangeState(TutorialStates.Green);
                }
            }
            else
            {
                if (_ingredientId == 1)
                {
                    ChangeState(TutorialStates.Purple);
                }

                if (_ingredientId == 2)
                {
                    ChangeState(TutorialStates.Red);
                }

                if (_ingredientId == 3)
                {
                    ChangeState(TutorialStates.Red);
                }

                if (_ingredientId == 4)
                {
                    ChangeState(TutorialStates.Purple);
                }

                if (_ingredientId == 5)
                {
                    ChangeState(TutorialStates.Green);
                }
            }
        }
    }
}