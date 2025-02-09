using Cysharp.Threading.Tasks;
using Src.Audio;
using Src.Common;
using Src.Control;
using UnityEngine;

namespace Src.Interaction.Sleep
{
    public class Sleep : MonoBehaviour, IInteractable
    {
        private const float SpeedMultiply = 0.3f;

        [SerializeField] private Transform _sleepPoint;
        [SerializeField] private Transform _wakeUpPoint;

        private TopDownController _characterController;
        private Rigidbody2D _characterRigidbody2D;
        private MusicManager _musicManager;
        private Blackout _blackout;

        private void Awake()
        {
            _characterController = FindFirstObjectByType<TopDownController>();
            _characterRigidbody2D = _characterController.GetComponent<Rigidbody2D>();
            _musicManager = FindFirstObjectByType<MusicManager>();
            _blackout = FindFirstObjectByType<Blackout>();
        }

        public void Use()
        {
            GoToSleepAsync().Forget();
        }

        private async UniTaskVoid GoToSleepAsync()
        {
            _characterRigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            _musicManager.ChangeMusic(MusicState.Sleep, false);
            await MoveToPointAsync(_sleepPoint);
            await _blackout.FadeIn(5);
            await UniTask.Delay(5000);
            await _blackout.FadeOut(3);
            await MoveToPointAsync(_wakeUpPoint);
            _musicManager.ChangeMusic(MusicState.Game, true);

            _characterRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _characterController.CanControl = true;
        }

        private async UniTask MoveToPointAsync(Transform point)
        {
            Vector2 targetPosition = point.position;

            while (Vector2.Distance(_characterRigidbody2D.position, targetPosition) > 0.1f)
            {
                var direction = (targetPosition - _characterRigidbody2D.position).normalized * SpeedMultiply;
                _characterController.Direction = direction;
                await UniTask.Yield();
            }

            _characterController.Direction = Vector2.zero;
        }
    }
}