using System.Threading.Tasks;
using Src.Control;
using UnityEngine;

namespace Src.Interaction.Sleep
{
    public class Sleep : MonoBehaviour, IInteractable
    {
        private const float SpeedMultiply = 0.3f;

        [SerializeField] private Transform _toPoint;

        private TopDownController _characterController;
        private Rigidbody2D _characterRigidbody2D;

        private void Awake()
        {
            _characterController = FindFirstObjectByType<TopDownController>();
            _characterRigidbody2D = _characterController.GetComponent<Rigidbody2D>();
        }

        public async void Use()
        {
            _characterRigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            await MoveToTarget();

            //_characterRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            //_characterController.CanControl = true;
        }

        private async Task MoveToTarget()
        {
            Vector2 targetPosition = _toPoint.position;

            while (Vector2.Distance(_characterRigidbody2D.position, targetPosition) > 0.1f)
            {
                var direction = (targetPosition - _characterRigidbody2D.position).normalized * SpeedMultiply;
                _characterController.Direction = direction;
                await Task.Yield();
                ;
            }

            _characterController.Direction = Vector2.zero;
        }
    }
}