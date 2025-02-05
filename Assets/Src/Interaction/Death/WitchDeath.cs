using Src.Control;
using UnityEngine;

namespace Src.Interaction.Death
{
    [RequireComponent(typeof(TopDownController))]
    public class WitchDeath : MonoBehaviour
    {
        private static readonly int Death = Animator.StringToHash("Death");

        [SerializeField] private Animator _characterAnimator;

        private TopDownController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<TopDownController>();
        }

        public void Kill()
        {
            _characterController.CanControl = false;
            _characterAnimator.SetTrigger(Death);
        }
    }
}