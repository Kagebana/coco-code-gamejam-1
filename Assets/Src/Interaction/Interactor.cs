using System.Collections.Generic;
using UnityEngine;

namespace Src.Interaction
{
    public class Interactor : MonoBehaviour
    {
        private readonly HashSet<Collider2D> _triggers = new();

        [SerializeField] private GameObject _mark;

        private void FixedUpdate()
        {
            _mark.SetActive(_triggers.Count != 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out TriggerZone _))
            {
                _triggers.Add(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _triggers.Remove(other);
        }
    }
}