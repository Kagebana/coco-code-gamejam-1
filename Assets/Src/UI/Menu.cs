using Src.Audio;
using Src.Interaction.Enter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Src.UI
{
	public class Menu : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private GameObject _mainMenu;
		[SerializeField] private GameObject _mainMenuButton;
		[SerializeField] private GameObject _settingsMenu;
		[SerializeField] private GameObject _settingsButton;
		[SerializeField] private TextMeshProUGUI _skipTutorialText;
		[SerializeField] private GameObject _creditsMenu;
		[SerializeField] private GameObject _creditsButton;

		private MenuStates _menuState = MenuStates.MainMenu;
		private PlayerInput _playerInput;
		private EventSystem _eventSystem;
		private GameObject _currentButton;
		private MusicManager _musicManager;
		private Enter _enter;

		private void Awake()
		{
			_eventSystem = FindAnyObjectByType<EventSystem>();
			_currentButton = _mainMenuButton;
			_musicManager = FindAnyObjectByType<MusicManager>();
			_enter = FindAnyObjectByType<Enter>();
		}

		private void OnEnable()
		{
			_playerInput = FindAnyObjectByType<PlayerInput>();
			_playerInput.actions["Navigate"].performed += ReturnFocus;
		}

		private void Start()
		{
			_musicManager.ChangeMusic(MusicState.Menu, true);
			LoadSave();
		}

		private void OnDisable()
		{
			if (!_playerInput)
			{
				return;
			}

			_playerInput.actions["Navigate"].performed -= ReturnFocus;
		}

		public bool SkipTutorial { get; private set; }

		public void OnPointerClick(PointerEventData eventData)
		{
			ChangeButton(_currentButton);
		}

		public void ToPlay()
		{
			_musicManager.StopMusic();
			_enter.ToStart();
			ChangeMenu(MenuStates.Closed);
		}

		public void ToSettings()
		{
			ChangeMenu(MenuStates.Settings);
		}

		public void ChangeSkipTutorial()
		{
			SkipTutorial = !SkipTutorial;
			_skipTutorialText.text = SkipTutorial ? "+" : "-";
			PlayerPrefs.SetInt("SkipTutorial", SkipTutorial ? 1 : 0);
			PlayerPrefs.Save();
		}

		public void ToCredits()
		{
			ChangeMenu(MenuStates.Credits);
		}

		public void ToExit()
		{
			Application.Quit();
		}

		public void ToBack()
		{
			ChangeMenu(MenuStates.MainMenu);
		}

		private void ReturnFocus(InputAction.CallbackContext callbackContext)
		{
			if (!_eventSystem.currentSelectedGameObject)
			{
				ChangeButton(_currentButton);
			}
		}

		private void ChangeMenu(MenuStates newState)
		{
			_menuState = newState;

			if (_menuState == MenuStates.MainMenu)
			{
				_settingsMenu.SetActive(false);
				_creditsMenu.SetActive(false);
				_mainMenu.SetActive(true);
				ChangeButton(_mainMenuButton);
			}

			if (_menuState == MenuStates.Settings)
			{
				_mainMenu.SetActive(false);
				_settingsMenu.SetActive(true);
				ChangeButton(_settingsButton);
			}

			if (_menuState == MenuStates.Credits)
			{
				_mainMenu.SetActive(false);
				_creditsMenu.SetActive(true);
				ChangeButton(_creditsButton);
			}

			if (_menuState == MenuStates.Closed)
			{
				_mainMenu.SetActive(false);
				_creditsMenu.SetActive(false);
			}
		}

		private void ChangeButton(GameObject button)
		{
			_currentButton = button;
			_eventSystem.SetSelectedGameObject(_currentButton);
		}

		private void LoadSave()
		{
			SkipTutorial = PlayerPrefs.GetInt("SkipTutorial", 0) == 1;
			_skipTutorialText.text = SkipTutorial ? "+" : "-";
		}
	}
}