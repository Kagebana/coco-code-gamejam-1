using System.Threading.Tasks;
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

        public async Task FadeIn(float duration = 1f)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
                await Task.Yield();
            }

            _canvasGroup.alpha = 1;
        }

        public async Task FadeOut(float duration = 1f)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / duration);
                await Task.Yield();
            }

            _canvasGroup.alpha = 0;
        }
    }
}