using UnityEngine;

namespace Src.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioClip _menu;
        [SerializeField] private AudioClip _game;
        [SerializeField] private AudioClip _sleep;
        [SerializeField] private AudioClip _death;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _audioSource.clip = _game;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
}