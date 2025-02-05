using UnityEngine;
using UnityEngine.UI;

namespace Src.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Blackout : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private SpriteRenderer _characterRenderer;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetWitchOnBlack()
        {
            _characterRenderer.sortingOrder = 31;
            _canvasGroup.alpha = 1;
        }
    }
}