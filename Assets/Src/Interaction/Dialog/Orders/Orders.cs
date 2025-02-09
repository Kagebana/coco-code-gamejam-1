using System;
using Cysharp.Threading.Tasks;
using Src.Audio;
using Src.Control;
using Src.Interaction.Death;
using Src.Interaction.Inventory;
using Src.UI;
using UnityEngine;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

namespace Src.Interaction.Dialog.Orders
{
    public class Orders : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioClip _poisonTutorialClip;
        [SerializeField] private AudioClip _extraPoisonTutorialClip;

        [SerializeField] private AudioClip[] _poisonClip;

        [TextArea(3, 10)] [SerializeField] private string[] _poisonTutorialLines;
        [SerializeField] private LocalizedString[] _localizedPoisonTutorialKeys;
        [TextArea(3, 10)] [SerializeField] private string[] _poisonTutorialFinishLines;
        [SerializeField] private LocalizedString[] _localizedPoisonTutorialFinishKeys;
        [TextArea(3, 10)] [SerializeField] private string[] _extraPoisonTutorialLines;
        [SerializeField] private LocalizedString[] _localizedExtraPoisonTutorialKeys;
        [TextArea(3, 10)] [SerializeField] private string[] _extraPoisonTutorialFinishLines;
        [SerializeField] private LocalizedString[] _localizedExtraPoisonTutorialFinishKeys;
        [TextArea(3, 10)] [SerializeField] private string[] _failFinishLines;
        [SerializeField] private LocalizedString[] _localizedFailFinishKeys;

        [SerializeField] private DialogueLinesGroup[] _poisonDialogues;
        [SerializeField] private LocalizedString[] _localizedPoisonKeys;
        [SerializeField] private DialogueLinesGroup[] _poisonFinishDialogues;
        [SerializeField] private LocalizedString[] _localizedPoisonFinishKeys;
        [SerializeField] private DialogueLinesGroup[] _extraPoisonDialogues;
        [SerializeField] private LocalizedString[] _localizedExtraPoisonKeys;
        [SerializeField] private DialogueLinesGroup[] _extraPoisonFinishDialogues;
        [SerializeField] private LocalizedString[] _localizedExtraPoisonFinishKeys;

        private OrderStates _orderState = OrderStates.NotTaken;
        private DialogManager _dialogManager;
        private bool _tutorialPoison;
        private bool _tutorialExtraPoison;
        private TopDownController _characterController;
        private Inventory.Inventory _characterInventory;
        private WitchDeath _witchDeath;
        private bool _orderType;
        private int _currentDialogId;
        private AudioClip _currentClip;
        private MusicManager _musicManager;
        private Menu _menu;

        private void Awake()
        {
            _dialogManager = FindAnyObjectByType<DialogManager>();
            _characterController = FindAnyObjectByType<TopDownController>();
            _characterInventory = FindAnyObjectByType<Inventory.Inventory>(FindObjectsInactive.Include);
            _witchDeath = FindAnyObjectByType<WitchDeath>();
            _musicManager = FindAnyObjectByType<MusicManager>();
            _currentClip = GetRandomElement(_poisonClip);
            _menu = FindAnyObjectByType<Menu>();
        }

        public Action OnPoisonTutorialStarted;
        public Action OnPoisonTutorialFinished;
        public Action OnExtraPoisonTutorialStarted;
        public Action OnExtraPoisonTutorialFinished;

        public Action OnPoisonSuccess;
        public Action OnExtraPoisonSuccess;

        public void Use()
        {
            if (FailureCheck())
            {
                return;
            }

            if (!_menu.SkipTutorial)
            {
                if (!_tutorialPoison)
                {
                    PoisonTutorial();
                    return;
                }

                if (!_tutorialExtraPoison)
                {
                    ExtraPoisonTutorial();
                    return;
                }
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

        private static async UniTask<string> GetLocalizedStringAsync(LocalizedString localizedString)
        {
            var handle = localizedString.GetLocalizedStringAsync();
            while (!handle.IsDone)
            {
                await UniTask.Yield();
            }

            return handle.Result;
        }

        private bool FailureCheck()
        {
            switch (_orderState)
            {
                case OrderStates.NotTaken:
                    return false;
                case OrderStates.Poison when _characterInventory.InventoryState == InventoryStates.ExtraPoison:
                case OrderStates.ExtraPoison when _characterInventory.InventoryState == InventoryStates.Poison:
                    _musicManager.StopMusic();
                    _dialogManager.OnDialogEnd += _witchDeath.Kill().Forget;
                    LoadLocalizedDialogue(_localizedFailFinishKeys, _failFinishLines).Forget();
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
                    LoadLocalizedDialogue(_localizedPoisonTutorialKeys, _poisonTutorialLines).Forget();
                    _dialogManager.StartDialogue(_poisonTutorialLines, _poisonTutorialClip);
                    _orderState = OrderStates.Poison;
                    OnPoisonTutorialStarted?.Invoke();
                    return;
                case false when _orderState == OrderStates.Poison &&
                                _characterInventory.InventoryState == InventoryStates.Poison:
                    _characterInventory.ChangeSlot(InventoryStates.Empty);
                    LoadLocalizedDialogue(_localizedPoisonTutorialFinishKeys, _poisonTutorialFinishLines).Forget();
                    _dialogManager.StartDialogue(_poisonTutorialFinishLines, _poisonTutorialClip);
                    _tutorialPoison = true;
                    _orderState = OrderStates.NotTaken;
                    OnPoisonTutorialFinished?.Invoke();
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
                    LoadLocalizedDialogue(_localizedExtraPoisonTutorialKeys, _extraPoisonTutorialLines).Forget();
                    _dialogManager.StartDialogue(_extraPoisonTutorialLines, _extraPoisonTutorialClip);
                    _orderState = OrderStates.ExtraPoison;
                    OnExtraPoisonTutorialStarted?.Invoke();
                    return;
                case false when _orderState == OrderStates.ExtraPoison &&
                                _characterInventory.InventoryState == InventoryStates.ExtraPoison:
                    _characterInventory.ChangeSlot(InventoryStates.Empty);
                    LoadLocalizedDialogue(_localizedExtraPoisonTutorialFinishKeys, _extraPoisonTutorialFinishLines)
                        .Forget();
                    _dialogManager.StartDialogue(_extraPoisonTutorialFinishLines, _extraPoisonTutorialClip);
                    _tutorialExtraPoison = true;
                    _orderState = OrderStates.NotTaken;
                    OnExtraPoisonTutorialFinished?.Invoke();
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
                            LoadLocalizedDialogues(_poisonDialogues, _localizedPoisonKeys).Forget();
                            _dialogManager.StartDialogue(_poisonDialogues[_currentDialogId].Lines, _currentClip);
                            return;
                        case OrderStates.ExtraPoison:
                            _currentDialogId = Random.Range(0, _poisonFinishDialogues.Length);
                            LoadLocalizedDialogues(_extraPoisonDialogues, _localizedExtraPoisonKeys).Forget();
                            _dialogManager.StartDialogue(_extraPoisonDialogues[_currentDialogId].Lines, _currentClip);
                            return;
                        case OrderStates.NotTaken:
                        default:
                            return;
                    }
                }
                case OrderStates.Poison when _characterInventory.InventoryState == InventoryStates.Poison:
                    _characterInventory.ChangeSlot(InventoryStates.Empty);
                    LoadLocalizedDialogues(_poisonFinishDialogues, _localizedPoisonFinishKeys).Forget();
                    _dialogManager.StartDialogue(_poisonFinishDialogues[_currentDialogId].Lines, _currentClip);
                    _orderState = OrderStates.NotTaken;
                    OnPoisonSuccess?.Invoke();
                    return;
                case OrderStates.ExtraPoison when _characterInventory.InventoryState == InventoryStates.ExtraPoison:
                    _characterInventory.ChangeSlot(InventoryStates.Empty);
                    LoadLocalizedDialogues(_extraPoisonFinishDialogues, _localizedExtraPoisonFinishKeys).Forget();
                    _dialogManager.StartDialogue(_extraPoisonFinishDialogues[_currentDialogId].Lines, _currentClip);
                    _orderState = OrderStates.NotTaken;
                    OnExtraPoisonSuccess?.Invoke();
                    return;
                default:
                    switch (_orderState)
                    {
                        case OrderStates.Poison:
                            _dialogManager.StartDialogue(_poisonDialogues[_currentDialogId].Lines, _currentClip);
                            return;
                        case OrderStates.ExtraPoison:
                            _dialogManager.StartDialogue(_extraPoisonDialogues[_currentDialogId].Lines, _currentClip);
                            return;
                        case OrderStates.NotTaken:
                        default:
                            return;
                    }
            }
        }

        private static async UniTaskVoid LoadLocalizedDialogue(LocalizedString[] localizedDialogueKeys,
            string[] dialogueLines)
        {
            if (localizedDialogueKeys.Length != dialogueLines.Length)
            {
                Debug.LogError(
                    $"[Orders] Количество ключей ({localizedDialogueKeys.Length}) не совпадает с количеством строк ({dialogueLines.Length})!");
                return;
            }

            for (var i = 0; i < localizedDialogueKeys.Length; i++)
            {
                dialogueLines[i] = await GetLocalizedStringAsync(localizedDialogueKeys[i]);
            }
        }

        private static async UniTaskVoid LoadLocalizedDialogues(DialogueLinesGroup[] dialogues,
            LocalizedString[] localizedKeys)
        {
            if (dialogues.Length != localizedKeys.Length)
            {
                Debug.LogError("Размеры массивов _poisonDialogues и _localizedPoisonKeys не совпадают!");
                return;
            }

            for (var i = 0; i < dialogues.Length; i++)
            {
                for (var j = 0; j < dialogues[i].Lines.Length; j++)
                {
                    dialogues[i].Lines[j] = await GetLocalizedStringAsync(localizedKeys[i]);
                }
            }
        }
    }
}