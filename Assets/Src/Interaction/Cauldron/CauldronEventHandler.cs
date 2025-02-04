using UnityEngine;

namespace Src.Interaction.Cauldron
{
    public class CauldronEventHandler : MonoBehaviour
    {
        private Cauldron _cauldron;

        private void Awake()
        {
            _cauldron = FindAnyObjectByType<Cauldron>();
        }

        public void AllowUse()
        {
            _cauldron.AllowUse();
        }
    }
}