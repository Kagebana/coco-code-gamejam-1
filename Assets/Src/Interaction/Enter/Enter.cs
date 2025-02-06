using System.Threading.Tasks;
using Src.Audio;
using Src.Common;
using Src.Control;
using UnityEngine;

namespace Src.Interaction.Enter
{
	public class Enter : MonoBehaviour
	{
		private const float SpeedMultiply = 0.3f;

		[SerializeField] private Transform _startPoint;
		[SerializeField] private GameObject _inventory;

		private TopDownController _characterController;
		private Rigidbody2D _characterRigidbody2D;
		private MusicManager _musicManager;

		private void Awake()
		{
			_characterController = FindFirstObjectByType<TopDownController>();
			_characterRigidbody2D = _characterController.GetComponent<Rigidbody2D>();
			_musicManager = FindFirstObjectByType<MusicManager>();
		}

		public async void ToStart()
		{
			_characterRigidbody2D.bodyType = RigidbodyType2D.Kinematic;

			await MoveToPointAsync(_startPoint);
			_musicManager.ChangeMusic(MusicState.Game, true);

			_inventory.gameObject.SetActive(true);
			_characterRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
			_characterController.CanControl = true;
		}

		private async Task MoveToPointAsync(Transform point)
		{
			Vector2 targetPosition = point.position;

			while (Vector2.Distance(_characterRigidbody2D.position, targetPosition) > 0.1f)
			{
				var direction = (targetPosition - _characterRigidbody2D.position).normalized * SpeedMultiply;
				_characterController.Direction = direction;
				await Task.Yield();
			}

			_characterController.Direction = Vector2.zero;
		}
	}
}