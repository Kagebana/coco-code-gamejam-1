using System;
using System.Threading.Tasks;
using Src.Interaction.Death;
using Src.Interaction.Dialog.Orders;
using TMPro;
using UnityEngine;

namespace Src.Score
{
	[RequireComponent(typeof(CanvasGroup))]
	public class ScoreViewer : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _poisonText;
		[SerializeField] private TextMeshProUGUI _extraPoisonText;

		private CanvasGroup _canvasGroup;
		private WitchDeath _witchDeath;
		private Action _onDeath;
		private int _poisonScore;
		private int _extraPoisonScore;
		private Orders _orders;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_witchDeath = FindFirstObjectByType<WitchDeath>();
			_orders = FindFirstObjectByType<Orders>();
		}

		private void OnEnable()
		{
			_witchDeath.OnDeath = _onDeath += () => _ = FadeIn();
			_orders.OnPoisonSuccess += AddPoisonScore;
			_orders.OnExtraPoisonSuccess += AddExtraPoisonScore;
		}

		private void OnDisable()
		{
			_witchDeath.OnDeath -= _onDeath;
			_orders.OnPoisonSuccess -= AddPoisonScore;
			_orders.OnExtraPoisonSuccess -= AddExtraPoisonScore;
		}

		private void AddPoisonScore()
		{
			_poisonScore++;
			_poisonText.text = _poisonScore.ToString("D2");
		}

		private void AddExtraPoisonScore()
		{
			_extraPoisonScore++;
			_extraPoisonText.text = _extraPoisonScore.ToString("D2");
		}

		private async Task FadeIn(float duration = 1f)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				_canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
				await Task.Yield();
			}

			_canvasGroup.alpha = 1;
		}
	}
}