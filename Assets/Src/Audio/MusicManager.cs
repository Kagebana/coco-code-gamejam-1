using System;
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
            ChangeMusic(MusicState.Game, true);
        }

        public void ChangeMusic(MusicState state, bool loop)
        {
            _audioSource.Stop();

            _audioSource.clip = state switch
            {
                MusicState.Menu => _menu,
                MusicState.Game => _game,
                MusicState.Sleep => _sleep,
                MusicState.Death => _death,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

            _audioSource.loop = loop;
            _audioSource.Play();
        }
    }
}