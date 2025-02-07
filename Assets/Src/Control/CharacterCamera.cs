using UnityEngine;

namespace Src.Control
{
	public class CharacterCamera : MonoBehaviour
	{
		private const float SmoothSpeed = 10f;
		private const int PixelsPerUnit = 40;

		private readonly Vector2 _minBounds = new(-3.77f, -0.07f);
		private readonly Vector2 _maxBounds = new(2.97f, 2.47f);

		private Transform _characterTransform;

		private void Awake()
		{
			_characterTransform = FindFirstObjectByType<TopDownController>().transform;
		}

		private void LateUpdate()
		{
			if (!_characterTransform)
			{
				return;
			}

			var desiredPosition = new Vector3(_characterTransform.position.x, _characterTransform.position.y,
				transform.position.z);
			var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed * Time.deltaTime);

			smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, _minBounds.x, _maxBounds.x);
			smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, _minBounds.y, _maxBounds.y);

			smoothedPosition.x = Mathf.Round(smoothedPosition.x * PixelsPerUnit) / PixelsPerUnit;
			smoothedPosition.y = Mathf.Round(smoothedPosition.y * PixelsPerUnit) / PixelsPerUnit;

			transform.position = smoothedPosition;
		}
	}
}